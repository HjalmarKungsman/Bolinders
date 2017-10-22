using Bolinders.Core.DataAccess;
using Bolinders.Core.Enums;
using Bolinders.Core.Models;
using Bolinders.Core.Models.Entities;
using Bolinders.Core.Models.SettingModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bolinders.Core.Services
{
    public class XmlToDbService : IXmlToDbService
    {
        private readonly ApplicationDbContext _context;
        private readonly Models.SettingModels.FtpSettings _ftpSettings;
        private readonly IImageService _image;
        private readonly IEmailSenderService _email;
        private readonly IHostingEnvironment _environment;

        public XmlToDbService(ApplicationDbContext context, IImageService image, IEmailSenderService email, IOptions<FtpSettings> ftpSettings, IHostingEnvironment environment)
        {
            _context = context;
            _image = image;
            _email = email;
            _ftpSettings = ftpSettings.Value;
            _environment = environment;
        }



        public async Task Run()
        {
            string _xmlFile = FtpDownload();

            List<VehicleXml> vehiclesAll = ParseXmlToObject(_xmlFile);

            List<VehicleXml> vehiclesUpdatedLastDay = SelectUpdatedVehicles(vehiclesAll);

            await SortVehicles(vehiclesUpdatedLastDay);
            return;

        }

        //Connects to FTP and downloads the XML-file
        private string FtpDownload()
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(_ftpSettings.FtpServer);
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(_ftpSettings.UserName, _ftpSettings.Password);

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("windows-1252"));
            string xmlFile = reader.ReadToEnd();

            reader.Close();
            response.Close();

            return xmlFile;
        }

        //Converts the XML-string to an IEnumerable-list of VehicleXml's
        private List<VehicleXml> ParseXmlToObject(string xmlFile)
        {
            Serializer ser = new Serializer();
            VehiclesXml vehiclesXml = ser.Deserialize<VehiclesXml>(xmlFile);

            return vehiclesXml.VehicleXml;
        }

        private List<VehicleXml> SelectUpdatedVehicles(List<VehicleXml> allVehicles)
        {
            TimeSpan epochTicks = new TimeSpan(new DateTime(1970, 1, 1).Ticks);

            // Change AddDays to a lower value if you want to go back more days
            TimeSpan unixTicks = new TimeSpan(DateTime.UtcNow.AddDays(-1).Ticks) - epochTicks;

            var vehiclesUpdatedLastDay = allVehicles.Select(n => n).Where(n => Double.Parse(n.Updated) > unixTicks.TotalSeconds).ToList();

            return vehiclesUpdatedLastDay;
        }

        private async Task SortVehicles(List<VehicleXml> vehicles)
        {
            List<Guid> addedVehicles = new List<Guid>();
            List<VehicleXml> vehiclesToRemove = new List<VehicleXml>();

            //Checks for edits and apply them.
            foreach (var vehicle in vehicles)
            {

                var existingVehicle = _context.Vehicles.Where(x => x.RegistrationNumber == vehicle.RegistrationNumber).Include(x => x.Images).FirstOrDefault();
                if (existingVehicle == null)
                {
                    break;
                }

                addedVehicles.Add(existingVehicle.Id);

                List<string> listOfImages = new List<string>();

                foreach (var image in vehicle.Images)
                {
                    listOfImages.Add(image.Url);
                }

                //remove old images from disk
                var directory = Path.Combine(_environment.WebRootPath, "images/uploads");
                foreach (var image in existingVehicle.Images)
                {
                   await _image.RemoveImageFromDisk(directory, image.ImageUrl);
                   _context.Images.Remove(image);
                }
                
                existingVehicle.Colour = vehicle.Colour;
                existingVehicle.FacilityId = vehicle.FacilityId;
                existingVehicle.Horsepowers = vehicle.Horsepowers;
                existingVehicle.Make = _context.Make.Where(x => x.Name == vehicle.Make).FirstOrDefault();
                existingVehicle.Mileage = Int32.Parse(vehicle.Milage);
                existingVehicle.Model = vehicle.Model;
                existingVehicle.ModelDescription = vehicle.ModelDescription;
                existingVehicle.RegistrationNumber = vehicle.RegistrationNumber;
                existingVehicle.Updated = FromUnixTime(Int32.Parse(vehicle.Updated));
                existingVehicle.Year = Int32.Parse(vehicle.Year);
                existingVehicle.Images = new List<Image>();

                if (Enum.TryParse(vehicle.BodyType, out BodyType bodyType))
                {
                    existingVehicle.BodyType = bodyType;
                }
                if (Enum.TryParse(vehicle.FuelType, out FuelType fuelType))
                {
                    existingVehicle.FuelType = fuelType;
                }
                if (Enum.TryParse(vehicle.Gearbox, out Gearbox gearbox))
                {
                    existingVehicle.Gearbox = gearbox;
                }

                if (vehicle.Leasable == "0" || vehicle.Leasable == "")
                {
                    existingVehicle.Leasable = false;
                }
                else
                {
                    existingVehicle.Leasable = true;
                }


                existingVehicle.Price = double.Parse(vehicle.Price, NumberStyles.Currency);


                var downloadedImages = _image.DownloadImagesFromURL(listOfImages);
                existingVehicle = _image.ImageBuilder(await downloadedImages, existingVehicle);



                if (vehicle.Equipment != null)
                {
                    var listOfEquipment = vehicle.Equipment.Split(",").ToList();
                    existingVehicle.Equipment = listOfEquipment.Select(x => new Equipment(x, existingVehicle)).ToList();
                }
                else
                {
                    existingVehicle.Equipment = null;
                }
                vehiclesToRemove.Add(vehicle);

                _context.Entry(existingVehicle).State = EntityState.Modified;
                _context.Entry(existingVehicle).Property(x => x.UrlId).IsModified = false;   
            }
            await _context.SaveChangesAsync();

            //removes already updated vehicles
            foreach (var item in vehiclesToRemove)
            {
                vehicles.Remove(item);
            }

            //Create new vehicles out of what's left in the list
            foreach (var vehicle in vehicles)
            {

                List<string> listOfImages = new List<string>();

                foreach (var image in vehicle.Images)
                {
                    listOfImages.Add(image.Url);
                }

                Vehicle newVehicle = new Vehicle
                {
                    Id = Guid.NewGuid(),
                    RegistrationNumber = vehicle.RegistrationNumber.ToUpper(),
                    Colour = vehicle.Colour,
                    Created = DateTime.UtcNow,
                    FacilityId = vehicle.FacilityId,
                    Horsepowers = vehicle.Horsepowers,
                    Make = _context.Make.Where(x => x.Name == vehicle.Make).First(),
                    Mileage = Int32.Parse(vehicle.Milage),
                    Model = vehicle.Model,
                    ModelDescription = vehicle.ModelDescription,
                    Updated = DateTime.UtcNow,
                    Year = Int32.Parse(vehicle.Year),
                    Images = new List<Image>()

                };

                newVehicle.Price = double.Parse(vehicle.Price, NumberStyles.Currency);


                if (Enum.TryParse(vehicle.BodyType, out BodyType bodyType))
                {
                    newVehicle.BodyType = bodyType;
                }
                if (Enum.TryParse(vehicle.FuelType, out FuelType fuelType))
                {
                    newVehicle.FuelType = fuelType;
                }
                if (Enum.TryParse(vehicle.Gearbox, out Gearbox gearbox))
                {
                    newVehicle.Gearbox = gearbox;
                }



                if (vehicle.Leasable == "0" || vehicle.Leasable == "")
                {
                    newVehicle.Leasable = false;
                }
                else
                {
                    newVehicle.Leasable = true;
                }


                if (newVehicle.Mileage > 0)
                {
                    newVehicle.Used = true;
                }
                else
                {
                    newVehicle.Used = false;
                }

                var downloadedImages = _image.DownloadImagesFromURL(listOfImages);
                newVehicle = _image.ImageBuilder(await downloadedImages, newVehicle);



                if (vehicle.Equipment != null)
                {
                    var listOfEquipment = vehicle.Equipment.Split(",").ToList();
                    newVehicle.Equipment = listOfEquipment.Select(x => new Equipment(x, newVehicle)).ToList();
                }
                else
                {
                    newVehicle.Equipment = null;
                }

                await _context.AddAsync(newVehicle);
                addedVehicles.Add(newVehicle.Id);
            }
            await _context.SaveChangesAsync();

            _email.SendImportNotification(addedVehicles);

            return;
        }

        private DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

    }
}
