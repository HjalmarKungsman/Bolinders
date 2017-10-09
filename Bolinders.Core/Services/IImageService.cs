using Bolinders.Core.Models.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bolinders.Core.Services
{
    public interface IImageService
    {
        List<string> DownloadImagesFromURL(List<string> images);
        Task<List<string>> UploadImages(ICollection<IFormFile> images);
        Task RemoveImageFromDisk(string directory, string fileName);
        Vehicle ImageBuilder(List<string> listOfImages, Vehicle vehicle);
    }
}
