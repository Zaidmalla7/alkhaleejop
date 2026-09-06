using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace alkhaleejop.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _env;

        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private readonly string[] _allowedMimeTypes = { "image/jpeg", "image/png", "image/webp" };

        public ImageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            if (file.Length > 5 * 1024 * 1024)
                throw new InvalidOperationException("حجم الملف يجب أن يكون أقل من 5 ميجابايت.");

            var originalExtension = Path.GetExtension(file.FileName).ToLower();

            if (!_allowedExtensions.Contains(originalExtension))
                throw new InvalidOperationException("امتداد الملف غير صالح. مسموح فقط بـ JPG, PNG, WEBP.");

            if (!_allowedMimeTypes.Contains(file.ContentType))
                throw new InvalidOperationException("نوع الملف (MIME type) غير مقبول أمنياً.");

            var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "glasses");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var fileName = Guid.NewGuid().ToString() + ".webp";
            var fullPath = Path.Combine(uploadPath, fileName);

            await using (var stream = file.OpenReadStream())
            {
                using (var image = await SixLabors.ImageSharp.Image.LoadAsync(stream))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(600, 600),
                        Mode = ResizeMode.Max
                    }));

                    var encoder = new WebpEncoder
                    {
                        Quality = 80 
                    };

                    using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        await image.SaveAsWebpAsync(fileStream, encoder);
                    }
                }
            }

            return "/uploads/glasses/" + fileName;
        }

        public void DeleteImage(string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return;

            var fullPath = Path.Combine(_env.WebRootPath, imagePath.TrimStart('/'));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}