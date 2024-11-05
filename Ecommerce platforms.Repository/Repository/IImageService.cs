using Ecommerce_platforms.Core.IRepository;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;


namespace Ecommerce_platforms.Repository.Repository
{
    public class ImageService : IImageService
    {
        public async Task<string> SaveImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Invalid file");
            }

            // Define the path where the file will be saved
            var filePath = Path.Combine("wwwroot/images", file.FileName);

            // Save the file to the local file system (or cloud storage)
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the URL of the uploaded image
            return $"https://yourdomain.com/images/{file.FileName}";
        }
    }
}
