using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Server.Services
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string subFolder, string? oldUrl = null);
        Task<string> UploadFileAsync(IFormFile file, string subFolder, string? oldUrl = null);
        
        bool DeleteFile(string? relativeUrl);
        Task<bool> DeleteFileAsync(string? relativeUrl);
        
        bool DeleteDirectory(string? relativeSubFolder);
        Task<bool> DeleteDirectoryAsync(string? relativeSubFolder);

        string GetPublicUrl(string path);
    }
}
