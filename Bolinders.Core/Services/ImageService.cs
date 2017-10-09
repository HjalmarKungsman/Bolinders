using Bolinders.Core.Models;
using Bolinders.Core.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Bolinders.Core.Services
{
    public class ImageService : IImageService
    {
        private readonly IHostingEnvironment _environment;

        public ImageService(IHostingEnvironment env)
        {
            _environment = env;
        }

        public List<string> DownloadImagesFromURL(List<string> images)
        {
            var uploadDirectory = Path.Combine(_environment.WebRootPath, "images/uploads");
            List<string> fileNames = new List<string>();
            
            foreach (var image in images)
            {
                var newImageName = Guid.NewGuid().ToString() + ".jpg";

                using (WebClient client = new WebClient())
                {
                    try
                    {
                        client.DownloadFile(new Uri(image), Path.Combine(uploadDirectory, newImageName));
                    }
                    catch (Exception)
                    {
                        break;
                    }
                    
                }
                var resizedImage = ImageResizer(newImageName);
                fileNames.Add(resizedImage);
                RemoveImageFromDisk(uploadDirectory, newImageName);
            }
            return fileNames;
        }
        

        public async Task<List<string>> UploadImages(ICollection<IFormFile> images)
        {
            var directory = Path.Combine(_environment.WebRootPath, "images/uploads");
            List<string> fileNames = new List<string>();


                if (images != null)
                {
                    foreach (var image in images)
                    {

                        using (var fileStream = new FileStream(Path.Combine(directory, image.FileName), FileMode.Create))
                        {
                            await image.CopyToAsync(fileStream);
                            fileStream.Close();
                        }

                        var resizedImage = ImageResizer(image.FileName);
                        fileNames.Add(resizedImage);
                        await RemoveImageFromDisk(directory, image.FileName);
                    }
                    return fileNames;
                }
            
            

            fileNames.Add("noimage.jpg");
            return fileNames;
        }

        public Task RemoveImageFromDisk(string directory, string fileName)
        {
            if (fileName == "noimage.jpg")
            {
                return Task.CompletedTask;
            }
            File.Delete(Path.Combine(directory, fileName));
            return Task.CompletedTask;
        }

        public Vehicle ImageBuilder(List<string> listOfImages, Vehicle vehicle)
        {
            for (int i = 0; i < listOfImages.Count; i++)
            {
                Models.Entities.Image newImage = new Models.Entities.Image(listOfImages[i], vehicle.Id);
                vehicle.Images.Add(newImage);
            }
            return vehicle;
        }

        //sets the image name to a guid in string format.
        private string FormatImageName(string fileName)
        {
            return Guid.NewGuid().ToString() + Path.GetExtension(fileName);
        }

        private string ImageResizer(string fileName)
        {
            var directory = Path.Combine(_environment.WebRootPath, "images/uploads");
            using (var image = System.Drawing.Image.FromFile(Path.Combine(directory, fileName)))
            using (var newImage = ScaleImage(image, 1024, 576))
            {
                var newFileName = FormatImageName(fileName);
                newImage.Save(Path.Combine(directory, newFileName), ImageFormat.Jpeg);

                return newFileName;
            }
        }
        private System.Drawing.Image ScaleImage(System.Drawing.Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }
    }
}
