using System;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Portfolio.Application.Abstraction.File;
using Portfolio.Application.Abstraction.Services;
using Portfolio.Application.Abstraction.Storage;
using Portfolio.Application.DTOs.File;
using Portfolio.Application.Repositories.SiteImageFile;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Services.File;

public class SiteImageFileService : ISiteImageFileService
{
    private readonly ISiteImageFileWriteRepository _writeRepo;
    private readonly ISiteImageFileReadRepository _readRepo;
    private readonly IMapper _mapper;
    private readonly IStorageService _storage;

    public SiteImageFileService(ISiteImageFileWriteRepository writeRepo,
        ISiteImageFileReadRepository readRepo,
        IMapper mapper,
        IStorageService storage)
    {
        _readRepo = readRepo;
        _writeRepo = writeRepo;
        _mapper=mapper;
        _storage = storage;

    }


    public async Task UploadAsync(SiteImageFileUploadDto dto, IFormFile file)
    {
        if (file is null || file.Length == 0)
            throw new ArgumentException("Dosya bulunamadı ve ya hatalı", nameof(file));

        var existing = await _readRepo.GetSingleAsync(x => x.SiteImageType == dto.SiteImageType);

        // Mevcut dosya varsa StorageService kullanarak sil
        if (existing != null 
            && !string.IsNullOrEmpty(existing.Path)
            && !string.IsNullOrEmpty(existing.FileName)
            && _storage.HasFile(existing.Path, existing.FileName))
        {
            await _storage.DeleteAsync(existing.Path, existing.FileName);
        }

        // StorageService’i kullanarak upload yap
        var uploadResult = await _storage.UploadAsync(dto.Path, file);
        var uploadedFile = uploadResult.FirstOrDefault();
        if (uploadResult == null || !uploadResult.Any())
            throw new InvalidOperationException("Dosya yükleme başarısız");
        if (existing != null)
        {
            existing.Path = uploadedFile.pathOrContainerName;
            existing.FileName = uploadedFile.fileName;
            _writeRepo.Update(existing);
        }
        else
        {
            var newImage = _mapper.Map<SiteImageFile>(dto);
            newImage.Path = uploadedFile.pathOrContainerName;
            newImage.FileName = uploadedFile.fileName;
            await _writeRepo.AddAsync(newImage);
        }
        await _writeRepo.SaveAsync();
    }
}
