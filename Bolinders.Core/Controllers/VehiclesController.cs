using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bolinders.Core.Models;
using Bolinders.Core.DataAccess;
using Microsoft.AspNetCore.Http;
using Bolinders.Core.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Bolinders.Core.Helpers;
using Bolinders.Core.Models.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Bolinders.Core.Enums;
using Bolinders.Core.Services;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using System.Net.Mail;
using Newtonsoft.Json;
using Bolinders.Core.Models.Entities;

namespace Bolinders.Core.Controllers
{
    [Authorize]
    public class VehiclesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IHostingEnvironment _environment;
        private static IHttpContextAccessor HttpContextAccessor;
        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }
        public VehiclesController(ApplicationDbContext context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public ViewResult GetXml()
        {
            XmlToDbService.Run();

            return View("Tada");
        }

        ////POST: Vehicles/List
        //[AllowAnonymous]
        //[HttpPost]
        //public IActionResult ListSearch(VehicleSearchModel formData = null, int page = 1, int pageLimit = 8)
        //{
        //    return RedirectToAction("List", formData, page, pageLimit);
        //}


        //GET: Vehicles/List
        [AllowAnonymous]
        //[HttpGet]
        public async Task<IActionResult> List(VehicleSearchModel formData = null, int page = 1, int pageLimit = 8)
        {
            //Sets the paginering object
            var paging = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = pageLimit,
                TotalItems = 0
            };

            //Sets the viewmodel with current page's vehicle list, paging info & searchmodels filter
            var vm = new VehicleListViewModel
            {
                Vehicles = null,
                Pager = paging,
                SeachModel = formData
            };

            if (!ModelState.IsValid)
            {
                return View("~/Views/Vehicles/List.cshtml", vm);
            }

            else
            {
                //Calculated how many pages to skip
                var toSkip = (page - 1) * pageLimit;

                //Get current page's list of vehicles
                var result = _context.Vehicles
                    .OrderByDescending(x => x.Created > x.Updated ? x.Created : x.Updated)
                    .Where(y => formData.SearchText == null ||
                        y.Make.Name.Contains(formData.SearchText) ||
                        y.Model.Contains(formData.SearchText) ||
                        y.ModelDescription.Contains(formData.SearchText))
                    .Where(y => formData.Used == null || y.Used.Equals(formData.Used))
                    .Where(z => formData.PriceFrom == null || z.Price >= formData.PriceFrom)
                    .Where(z => formData.PriceTo == null || z.Price <= formData.PriceTo)
                    .Where(z => formData.YearFrom == null || z.Year >= formData.YearFrom)
                    .Where(z => formData.YearTo == null || z.Year <= formData.YearTo)
                    .Where(z => formData.MileageFrom == null || z.Mileage >= formData.MileageFrom)
                    .Where(z => formData.MileageTo == null || z.Mileage <= formData.MileageTo)
                    .Where(z => formData.Make == null || z.MakeId.Equals(formData.Make))
                    .Where(z => formData.BodyType == null || z.BodyType.Equals(formData.BodyType))
                    .Where(z => formData.Gearbox == null || z.Gearbox.Equals(formData.Gearbox))
                    .Where(z => formData.FuelType == null || z.FuelType.Equals(formData.FuelType))
                    .Skip(toSkip)
                    .Take(pageLimit)
                    .Include(v => v.Make)
                    .Include(v => v.Images)
                    .Include(v => v.Equipment)
                    .AsQueryable();

                //Calculates total hits in db
                var countResult = _context.Vehicles
                    .Where(y => formData.SearchText == null ||
                        y.Make.Name.Contains(formData.SearchText) ||
                        y.Model.Contains(formData.SearchText) ||
                        y.ModelDescription.Contains(formData.SearchText))
                    .Where(y => formData.Used == null || y.Used.Equals(formData.Used))
                    .Where(z => formData.PriceFrom == null || z.Price >= formData.PriceFrom)
                    .Where(z => formData.PriceTo == null || z.Price <= formData.PriceTo)
                    .Where(z => formData.YearFrom == null || z.Year >= formData.YearFrom)
                    .Where(z => formData.YearTo == null || z.Year <= formData.YearTo)
                    .Where(z => formData.MileageFrom == null || z.Mileage >= formData.MileageFrom)
                    .Where(z => formData.MileageTo == null || z.Mileage >= formData.MileageTo)
                    .Where(z => formData.Make == null || z.MakeId.Equals(formData.Make))
                    .Where(z => formData.BodyType == null || z.BodyType.Equals(formData.BodyType))
                    .Where(z => formData.Gearbox == null || z.Gearbox.Equals(formData.Gearbox))
                    .Where(z => formData.FuelType == null || z.FuelType.Equals(formData.FuelType))
                    .Count();

                var itemsFinal = await result.ToListAsync();

                //updates the paginering object
                paging.TotalItems = countResult;

                //updates the viewmodel with current page's vehicle list, paging info
                vm.Vehicles = itemsFinal;
                vm.Pager = paging;

                return View("~/Views/Vehicles/List.cshtml", vm);
            }

        }

        //GET: Vehicles
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Vehicles
                .Include(v => v.Facility)
                .Include(v => v.Make)
                .OrderBy(v => v.ModelDescription)
                .OrderBy(v => v.Model)
                .OrderBy(v => v.Make.Name);
            return View(await applicationDbContext.ToListAsync());
        }

        //POST: ShareVehicles
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ShareVehicle(Guid id, string reciever)
        {

            var vehicle = _context.Vehicles.Include(x => x.Make).Where(x => x.Id == id).First();
            var absUrl = string.Format("{0}://{1}", Request.Scheme,
            Request.Host);

            var emailSender = await EmailSenderService.SendEmailWithSharedVehicle(reciever, vehicle, absUrl);

            if (emailSender == SmtpStatusCode.Ok)
            {
                return Ok("Ditt meddelande har skickats.");
            }
            else
            {
                return BadRequest("Ett fel har uppstått se över dina uppgifter och testa igen.");
            }
        }

        // GET: Vehicles/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid? id, string vehicleLinkId = null)
        {

            if (id == null && vehicleLinkId == null)
            {
                return NotFound();
            }
            else if (id != null)
            {
                var vehicle = await _context.Vehicles
                    .Include(v => v.Facility)
                    .Include(v => v.Make)
                    .Include(v => v.Images)
                    .Include(v => v.Equipment)
                    .SingleOrDefaultAsync(m => m.Id == id);
                if (vehicle == null)
                {
                    return NotFound();
                }



                return View(vehicle);
            }
            else if (vehicleLinkId != null)
            {
                var lastSign = vehicleLinkId.LastIndexOf("-");
                int vehicleId = Int32.Parse(vehicleLinkId.Substring(lastSign + 1));


                var vehicle = await _context.Vehicles
                    .Include(v => v.Facility)
                    .Include(v => v.Make)
                    .Include(v => v.Images)
                    .Include(v => v.Equipment)
                    .SingleOrDefaultAsync(m => m.UrlId == vehicleId);
                if (vehicle == null)
                {
                    return NotFound();
                }

                //kollar relaterade makeids på aktuellt fordon, jämför pris och tar ut max 4st. 
                var relatedVehicles = _context.Vehicles
                    .Include(v => v.Make)
                    .Where(b => b.MakeId == vehicle.MakeId)
                    .Where(b => b.Price >= vehicle.Price)
                    .Where(b => b.Id != vehicle.Id).Take(4)
                    .Include(v => v.Images)
                    
                    .ToList();

                ViewBag.relatedVehicles = relatedVehicles;

                return View(vehicle);
            }
            else
            {
                return NotFound();
            }


        }

        // GET: Vehicles/Create
        public IActionResult Create()
        {
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "Id", "Name");
            ViewData["MakeId"] = new SelectList(_context.Make, "Id", "Name");
            return View();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RegistrationNumber,MakeId,Model,ModelDescription,Year,Mileage,Price,BodyType,Colour,Gearbox,FuelType,Horsepowers,Used,FacilityId,Leasable,Created,Updated,Images,Equipment")] VehicleViewModel vehicle)
        {
            if (ModelState.IsValid)
            {

                var listOfImages = await ImageHelpers.UploadImages(vehicle.Images, _environment);

                Vehicle newVehicle = new Vehicle
                {
                    Id = Guid.NewGuid(),
                    RegistrationNumber = vehicle.RegistrationNumber.ToUpper(),
                    BodyType = vehicle.BodyType,
                    Colour = vehicle.Colour,
                    Created = DateTime.UtcNow,
                    Facility = vehicle.Facility,
                    FacilityId = vehicle.FacilityId,
                    FuelType = vehicle.FuelType,
                    Gearbox = vehicle.Gearbox,
                    Horsepowers = vehicle.Horsepowers,
                    Leasable = vehicle.Leasable,
                    Make = vehicle.Make,
                    MakeId = vehicle.MakeId,
                    Mileage = vehicle.Mileage,
                    Model = vehicle.Model,
                    ModelDescription = vehicle.ModelDescription,
                    Price = vehicle.Price,
                    Updated = DateTime.UtcNow,
                    Used = vehicle.Used,
                    Year = vehicle.Year,
                    Images = new List<Image>(),
                    Equipment = new List<Equipment>()
                };

                newVehicle = ImageHelpers.ImageBuilder(listOfImages, newVehicle);

                if (vehicle.Equipment != null)
                {
                    newVehicle = EquipmentHelpers.EquipmentBuilder(vehicle.Equipment, newVehicle);
                }

                _context.Add(newVehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "Id", "Name", vehicle.FacilityId);
            ViewData["MakeId"] = new SelectList(_context.Make, "Id", "Name", vehicle.MakeId);
            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles.Include(x => x.Images).Include(x => x.Equipment).SingleOrDefaultAsync(m => m.Id == id);

            if (vehicle == null)
            {
                return NotFound();
            }

            VehicleEditModel vehicleEditing = new VehicleEditModel
            {
                RegistrationNumber = vehicle.RegistrationNumber,
                Make = vehicle.Make,
                MakeId = vehicle.MakeId,
                Model = vehicle.Model,
                ModelDescription = vehicle.ModelDescription,
                Year = vehicle.Year,
                Mileage = vehicle.Mileage,
                Price = vehicle.Price,
                BodyType = vehicle.BodyType,
                Colour = vehicle.Colour,
                Gearbox = vehicle.Gearbox,
                FuelType = vehicle.FuelType,
                Horsepowers = vehicle.Horsepowers,
                Used = vehicle.Used,
                FacilityId = vehicle.FacilityId,
                Facility = vehicle.Facility,
                ImageList = vehicle.Images,
                Leasable = vehicle.Leasable,
                Created = vehicle.Created,
                Updated = vehicle.Updated,
                Equipment = vehicle.Equipment
            };
            if (vehicleEditing.Equipment != null)
            {
                vehicleEditing.EquipmentString = new List<string>();
                foreach (var i in vehicleEditing.Equipment)
                {
                    vehicleEditing.EquipmentString.Add(i.Value);
                }
            }


            ViewData["FacilityId"] = new SelectList(_context.Facilities, "Id", "Name");
            ViewData["MakeId"] = new SelectList(_context.Make, "Id", "Name");

            return View(vehicleEditing);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,RegistrationNumber,MakeId,Model,ModelDescription,Year,Mileage,Price,BodyType,Colour,Gearbox,FuelType,Horsepowers,FacilityId,ImageList,Images,Used,Leasable,Created,Updated,EquipmentString")] VehicleEditModel vehicle)
        {

            if (id != vehicle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingVehicle = 
                    await _context.Vehicles.Include(x => x.Images).Include(x => x.Equipment).SingleOrDefaultAsync(m => m.Id == id);
                existingVehicle.Id = vehicle.Id;
                existingVehicle.RegistrationNumber = vehicle.RegistrationNumber;
                existingVehicle.BodyType = vehicle.BodyType;
                existingVehicle.Colour = vehicle.Colour;
                existingVehicle.Created = vehicle.Created;
                existingVehicle.Facility = vehicle.Facility;
                existingVehicle.FacilityId = vehicle.FacilityId;
                existingVehicle.FuelType = vehicle.FuelType;
                existingVehicle.Gearbox = vehicle.Gearbox;
                existingVehicle.Horsepowers = vehicle.Horsepowers;
                existingVehicle.Leasable = vehicle.Leasable;
                existingVehicle.Make = vehicle.Make;
                existingVehicle.MakeId = vehicle.MakeId;
                existingVehicle.Mileage = vehicle.Mileage;
                existingVehicle.Model = vehicle.Model;
                existingVehicle.ModelDescription = existingVehicle.ModelDescription;
                existingVehicle.Price = vehicle.Price;
                existingVehicle.Used = vehicle.Used;
                existingVehicle.Year = vehicle.Year;

                if (vehicle.EquipmentString != null)
                {
                    existingVehicle.Equipment = vehicle.EquipmentString.Select(x => new Equipment(x, existingVehicle)).ToList();
                }
                else
                {
                    existingVehicle.Equipment = null;
                }
                      

                if (vehicle.ImageList != null)
                {
                    foreach (var item in vehicle.ImageList)
                    {
                        existingVehicle.Images.Add(item);
                    }
                }
                if (vehicle.Images != null)
                {
                    var listOfImages = await ImageHelpers.UploadImages(vehicle.Images, _environment);
                    existingVehicle = ImageHelpers.ImageBuilder(listOfImages, existingVehicle);
                }

                if (existingVehicle.Images.Count == 0 && vehicle.ImageList == null)
                {
                    List<string> fillerImage = new List<string>
                    {
                        "noimage.jpg"
                    };

                    existingVehicle = ImageHelpers.ImageBuilder(fillerImage, existingVehicle);
                }



                try
                {
                    existingVehicle.Updated = DateTime.UtcNow;

                    _context.Entry(existingVehicle).State = EntityState.Modified;
                    _context.Entry(existingVehicle).Property(x => x.UrlId).IsModified = false;
                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FacilityId"] = new SelectList(_context.Facilities, "Id", "Id", vehicle.FacilityId);
            ViewData["MakeId"] = new SelectList(_context.Make, "Id", "Id", vehicle.MakeId);

            return View(vehicle);
        }

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .Include(v => v.Facility)
                .Include(v => v.Make)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelected(List<Guid> selectedVehicles)
        {
            if (selectedVehicles == null)
            {
                return NotFound();
            }
            foreach (var i in selectedVehicles)
            {
                var vehicle = await _context.Vehicles.SingleOrDefaultAsync(m => m.Id == i);
                _context.Vehicles.Remove(vehicle);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var vehicle = await _context.Vehicles.SingleOrDefaultAsync(m => m.Id == id);
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RemoveImage(string imageId, string imagelink)
        {
            var imgGuid = new Guid(imageId);
            var image = await _context.Images.SingleOrDefaultAsync(m => m.Id == imgGuid);
            var directory = Path.Combine(_environment.WebRootPath, "images/uploads");
            await ImageHelpers.RemoveImageFromDisk(directory, imagelink);
            _context.Images.Remove(image);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(Guid id)
        {
            return _context.Vehicles.Any(e => e.Id == id);
        }
    }
}
