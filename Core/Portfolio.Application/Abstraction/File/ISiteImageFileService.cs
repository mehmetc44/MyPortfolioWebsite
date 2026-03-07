using System;
using Microsoft.AspNetCore.Http;
using Portfolio.Application.DTOs.File;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Abstraction.File;

public interface ISiteImageFileService : IFileService<SiteImageFile,SiteImageFileUploadDto>
{
    Task<SiteImageFile> GetSiteImageFileByTypeAsync(SiteImageType type);
}
