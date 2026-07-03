using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Server.Services
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string subFolder, string? oldUrl = null);
    }
}
