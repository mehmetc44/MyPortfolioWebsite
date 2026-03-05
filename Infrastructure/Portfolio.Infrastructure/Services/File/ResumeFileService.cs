using System;
using Microsoft.AspNetCore.Http;
using Portfolio.Application.Abstraction.File;
using Portfolio.Application.DTOs.File;
using Portfolio.Domain.Entities;

namespace Portfolio.Infrastructure.Services.File;

public class ResumeFileService : IResumeFileService
{
    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<ResumeFile>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ResumeFile> GetSingleAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task UploadAsync(ResumeFileUploadDto dto, IFormFile file)
    {
        throw new NotImplementedException();
    }
}
