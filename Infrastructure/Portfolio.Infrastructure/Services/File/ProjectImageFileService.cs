using System;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Abstraction.File;
using Portfolio.Application.Abstraction.Storage;
using Portfolio.Application.DTOs.File;
using Portfolio.Application.Repositories;
using Portfolio.Application.Repositories.ProjectImageFile;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Services.File;

public class ProjectImageFileService : IProjectImageFileService
{
    private readonly IProjectImageFileWriteRepository _writeRepo;
    private readonly IProjectImageFileReadRepository _readRepo;
    private readonly IMapper _mapper;
    private readonly IStorageService _storage;

    public ProjectImageFileService(IProjectImageFileWriteRepository writeRepo,
        IProjectImageFileReadRepository readRepo,
        IMapper mapper,
        IStorageService storage)
    {
        _readRepo = readRepo;
        _writeRepo = writeRepo;
        _mapper = mapper;
        _storage = storage;

    }
    
    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<ProjectImageFile>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<List<ProjectImageFile>> GetImagesByProjectIdAsync(Guid projectId)
    {
        if (projectId == Guid.Empty)
            throw new ArgumentException("Geçerli bir Proje ID'si belirtilmelidir.");
        return await _readRepo.GetWhere(x => x.Project.Id == projectId).ToListAsync();
    }

    public Task<ProjectImageFile> GetSingleAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task UploadAsync(ProjectImageFileUploadDto dto, IFormFile file)
    {
        throw new NotImplementedException();
    }
}
