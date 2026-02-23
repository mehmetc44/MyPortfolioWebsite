using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Portfolio.Application.Abstraction.Storage.Local;

namespace Portfolio.Infrastructure.Services.Storage.Local;

public class LocalStorage : Storage, ILocalStorage
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    public LocalStorage(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }
    public Task DeleteAsync(string path, string fileName)
    {
        File.Delete(Path.Combine(path, fileName));
        return Task.CompletedTask;
    }

    public List<string> GetFiles(string path)
    {
        DirectoryInfo directory = new(path);
        return directory.GetFiles().Select(f => f.Name).ToList();
    }

    public bool HasFile(string path, string fileName)
        => File.Exists(Path.Combine(path, fileName));
    private async Task<bool> CopyFileAsync(string path, IFormFile file)
    {
        await using FileStream fileStream = new(path, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, useAsync: false);
        await file.CopyToAsync(fileStream);
        await fileStream.FlushAsync();
        return true;
    }

    public async Task<List<(string fileName, string pathOrContainerName)>> UploadAsync(string path, IFormFileCollection files)
    {
        string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, path);

        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        List<(string fileName, string path)> datas = new();

        foreach (IFormFile file in files)
        {
            string fileNewName = await FileRenameAsync(path, file.Name, HasFile);
            string fullPath = Path.Combine(uploadPath, fileNewName);

            await CopyFileAsync(fullPath, file);
            datas.Add((fileNewName, Path.Combine(path, fileNewName)));
        }

        return datas;
    }
}
