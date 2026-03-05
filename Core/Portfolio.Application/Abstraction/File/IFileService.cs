using System;
using Microsoft.AspNetCore.Http;
using Portfolio.Application.DTOs.File;
using Portfolio.Domain.Entities;
namespace Portfolio.Application.Abstraction.File;

public interface IFileService<TEntity, TUploadDto>
    where TEntity : Domain.Entities.File, new()
    where TUploadDto : class
{
    Task<TEntity> GetSingleAsync(Guid id);
    Task<List<TEntity>> GetAllAsync();
    Task UploadAsync(TUploadDto dto, IFormFile file);
    Task DeleteAsync(Guid id);
}
