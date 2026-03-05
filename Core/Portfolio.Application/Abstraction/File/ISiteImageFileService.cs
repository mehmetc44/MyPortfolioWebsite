using System;
using Microsoft.AspNetCore.Http;
using Portfolio.Application.DTOs.File;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Abstraction.File;

public interface ISiteImageFileService : IFileService
{
    public Task UploadAsync(SiteImageFileUploadDto dto, IFormFile file);
}
