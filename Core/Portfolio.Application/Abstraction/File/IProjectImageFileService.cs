using System;
using Portfolio.Application.DTOs.File;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Abstraction.File;

public interface IProjectImageFileService : IFileService<ProjectImageFile,ProjectImageFileUploadDto>
{
    Task<List<ProjectImageFile>> GetImagesByProjectIdAsync(Guid projectId);
}
