using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;
using Server.Models;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Server.CQRS.Projects
{
    public record UpdateProjectCommand(string Id, ProjectEntity UpdatedProject) : IRequest<ProjectEntity>;

    public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ProjectEntity>
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;
        private readonly IMemoryCache _cache;

        public UpdateProjectCommandHandler(AppDbContext context, IFileService fileService, IMemoryCache cache)
        {
            _context = context;
            _fileService = fileService;
            _cache = cache;
        }

        public async Task<ProjectEntity> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            var id = request.Id;
            var updatedProject = request.UpdatedProject;

            if (id != updatedProject.Id)
            {
                throw new ArgumentException("ID uyuşmazlığı.");
            }

            var existing = await _context.Projects.FindAsync(new object[] { id }, cancellationToken);
            if (existing == null)
            {
                throw new KeyNotFoundException("Güncellenecek proje bulunamadı.");
            }

            DeleteRemovedPhysicalImages(existing.ImagesJson, updatedProject.ImagesJson);

            // Map values
            existing.Title_TR = updatedProject.Title_TR;
            existing.Title_EN = updatedProject.Title_EN;
            existing.Title_DE = updatedProject.Title_DE;
            existing.Category = updatedProject.Category;
            existing.Date = updatedProject.Date;
            existing.Client = updatedProject.Client;
            existing.SubTag_TR = updatedProject.SubTag_TR;
            existing.SubTag_EN = updatedProject.SubTag_EN;
            existing.SubTag_DE = updatedProject.SubTag_DE;
            existing.Description_TR = updatedProject.Description_TR;
            existing.Description_EN = updatedProject.Description_EN;
            existing.Description_DE = updatedProject.Description_DE;
            existing.Tech = updatedProject.Tech;
            existing.RepoUrl = updatedProject.RepoUrl;
            existing.DemoUrl = updatedProject.DemoUrl;
            existing.ImagesJson = updatedProject.ImagesJson;
            existing.DetailText_TR = updatedProject.DetailText_TR;
            existing.DetailText_EN = updatedProject.DetailText_EN;
            existing.DetailText_DE = updatedProject.DetailText_DE;

            _context.Entry(existing).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);

            ClearProjectsCache(id);

            return existing;
        }

        private void ClearProjectsCache(string id)
        {
            _cache.Remove("Projects_tr");
            _cache.Remove("Projects_en");
            _cache.Remove("Projects_de");
            _cache.Remove($"Project_{id}_tr");
            _cache.Remove($"Project_{id}_en");
            _cache.Remove($"Project_{id}_de");
        }

        private void DeleteRemovedPhysicalImages(string? oldJson, string? newJson)
        {
            if (string.IsNullOrWhiteSpace(oldJson)) return;

            try
            {
                var oldList = System.Text.Json.JsonSerializer.Deserialize<List<string>>(oldJson) ?? new List<string>();
                var newList = string.IsNullOrWhiteSpace(newJson) 
                    ? new List<string>() 
                    : (System.Text.Json.JsonSerializer.Deserialize<List<string>>(newJson) ?? new List<string>());

                var removedFiles = oldList.Where(oldImg => !newList.Contains(oldImg)).ToList();

                foreach (var imgPath in removedFiles)
                {
                    _fileService.DeleteFile(imgPath);
                }
            }
            catch
            {
                // Fail silently
            }
        }
    }
}
