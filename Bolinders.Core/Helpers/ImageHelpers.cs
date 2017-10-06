using Bolinders.Core.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bolinders.Core.Helpers
{
    public class ImageHelpers
    {
        public static async Task<List<string>> UploadImages(ICollection<IFormFile> images, IHostingEnvironment _environment)
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

                        var resizedImage = ImageResizer(image.FileName, _environment);
                        fileNames.Add(resizedImage);
                        await RemoveImageFromDisk(directory, image.FileName);
                    }
                    return fileNames;
                }
            
            

            fileNames.Add("noimage.jpg");
            return fileNames;
        }

        public static Task RemoveImageFromDisk(string directory, string fileName)
        {
            if (fileName == "noimage.jpg")
            {
                return Task.CompletedTask;
            }
            File.Delete(Path.Combine(directory, fileName));
            return Task.CompletedTask;
        }

        public static Vehicle ImageBuilder(List<string> listOfImages, Vehicle vehicle)
        {
            for (int i = 0; i < listOfImages.Count; i++)
            {
                Models.Image newImage = new Models.Image(listOfImages[i], vehicle.Id);
                vehicle.Images.Add(newImage);
            }
            return vehicle;
        }

        //sets the image name to a guid in string format.
        private static string FormatImageName(string fileName)
        {
            return Guid.NewGuid().ToString() + Path.GetExtension(fileName);
        }

        private static string ImageResizer(string fileName, IHostingEnvironment _environment)
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
        public static System.Drawing.Image ScaleImage(System.Drawing.Image image, int maxWidth, int maxHeight)
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
