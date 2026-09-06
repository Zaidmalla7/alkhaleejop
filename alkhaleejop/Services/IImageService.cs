using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace alkhaleejop.Services
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile file);
        void DeleteImage(string imagePath);
    }
}