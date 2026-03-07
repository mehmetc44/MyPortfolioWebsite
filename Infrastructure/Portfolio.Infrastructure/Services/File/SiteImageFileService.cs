
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Abstraction.File;
using Portfolio.Application.Abstraction.Storage;
using Portfolio.Application.DTOs.File;
using Portfolio.Application.Repositories.SiteImageFile;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;

namespace Portfolio.Infrastructure.Services.File;

public class SiteImageFileService : ISiteImageFileService
{
    private readonly ISiteImageFileWriteRepository _writeRepo;
    private readonly ISiteImageFileReadRepository _readRepo;
    private readonly IMapper _mapper;
    private readonly IStorageService _storage;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public SiteImageFileService(ISiteImageFileWriteRepository writeRepo,
        ISiteImageFileReadRepository readRepo,
        IMapper mapper,
        IStorageService storage,
        IWebHostEnvironment webHostEnvironment)
    {
        _readRepo = readRepo;
        _writeRepo = writeRepo;
        _mapper = mapper;
        _storage = storage;
        _webHostEnvironment=webHostEnvironment;
    }

    public async Task<List<SiteImageFile>> GetAllAsync()
    {
        return await _readRepo.GetAll().ToListAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Geçerli bir ID gönderilmelidir.", nameof(id));
        var existingImage = await _readRepo.GetSingleAsync(x => x.Id == id);

        if (existingImage == null)
            throw new Exception("Silinmek istenen görsel bulunamadı.");
        if (!string.IsNullOrEmpty(existingImage.Path) && !string.IsNullOrEmpty(existingImage.FileName))
        {
            if (_storage.HasFile(existingImage.Path, existingImage.FileName))
            {
                await _storage.DeleteAsync(existingImage.Path, existingImage.FileName);
            }
        }
        _writeRepo.Remove(existingImage);
        await _writeRepo.SaveAsync();
    }

    public async Task<SiteImageFile> GetSingleAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Geçerli bir ID gönderilmelidir.", nameof(id));
        var image = await _readRepo.GetSingleAsync(x => x.Id == id);
        return image;
    }

    public async Task UploadAsync(SiteImageFileUploadDto dto, IFormFile file)
    {
        if (file is null || file.Length == 0)
            throw new ArgumentException("Dosya bulunamadı ve ya hatalı", nameof(file));

        var existing = await _readRepo.GetSingleAsync(x => x.SiteImageType == dto.SiteImageType);

        if (existing != null
            && !string.IsNullOrEmpty(existing.Path)
            && !string.IsNullOrEmpty(existing.FileName)
            && _storage.HasFile(existing.Path, existing.FileName))
        {
            await _storage.DeleteAsync(existing.Path, existing.FileName);
        }
        var uploadResult = await _storage.UploadAsync(dto.Path, file);
        var uploadedFile = uploadResult.FirstOrDefault();
        if (uploadResult == null || !uploadResult.Any())
            throw new InvalidOperationException("Dosya yükleme başarısız");
        if (existing != null)
        {
            existing.Path = uploadedFile.pathOrContainerName;
            existing.FullPath = Path.Combine(_webHostEnvironment.WebRootPath,existing.Path);
            existing.FileName = uploadedFile.fileName;
            _writeRepo.Update(existing);
        }
        else
        {
            var newImage = _mapper.Map<SiteImageFile>(dto);
            newImage.Path = uploadedFile.pathOrContainerName;
            newImage.FullPath = Path.Combine(_webHostEnvironment.WebRootPath,newImage.Path);
            newImage.FileName = uploadedFile.fileName;
            await _writeRepo.AddAsync(newImage);
        }
        await _writeRepo.SaveAsync();
    }

    public async Task<SiteImageFile> GetSiteImageFileByTypeAsync(SiteImageType type)
    {
        var image = await _readRepo.GetSingleAsync(x => x.SiteImageType == type);
        if (image == null)
            throw new Exception("Belirtilen türde görsel bulunamadı.");
        return image;
    }
}
