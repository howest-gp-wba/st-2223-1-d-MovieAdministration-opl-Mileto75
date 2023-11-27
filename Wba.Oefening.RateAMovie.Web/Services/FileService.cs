using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Wba.Oefening.RateAMovie.Web.Services
{
    public class FileService : IFileService
    {
        public async Task<string> AddOrUpdateFile(IFormFile file, string subPath, IWebHostEnvironment
            webHostEnvironment, string fileName = "")
        {
            if (fileName == "")
            {
                fileName = $"{Guid.NewGuid()}_{Path.GetExtension(file.FileName)}";
            }
            string filePath = Path
                .Combine(webHostEnvironment.WebRootPath, "images", subPath, fileName);
            //store image
            using (FileStream fileStream = new(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return fileName;
        }
    }
}
