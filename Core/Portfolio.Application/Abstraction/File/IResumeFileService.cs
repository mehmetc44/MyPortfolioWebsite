using System;
using Portfolio.Application.DTOs.File;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Abstraction.File;

public interface IResumeFileService:IFileService<ResumeFile, ResumeFileUploadDto>
{

}
