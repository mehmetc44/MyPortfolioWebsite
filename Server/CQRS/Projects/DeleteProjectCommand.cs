using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Server.CQRS.Projects
{
    public record DeleteProjectCommand(string Id) : IRequest;

    public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand>
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;
        private readonly IMemoryCache _cache;

        public DeleteProjectCommandHandler(AppDbContext context, IFileService fileService, IMemoryCache cache)
        {
            _context = context;
            _fileService = fileService;
            _cache = cache;
        }

        public async Task Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var id = request.Id;
            var project = await _context.Projects.FindAsync(new object[] { id }, cancellationToken);
            if (project == null)
            {
                throw new KeyNotFoundException("Silinecek proje bulunamadı.");
            }

            DeleteRemovedPhysicalImages(project.ImagesJson, null);
            _fileService.DeleteDirectory($"projects/{id}");

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync(cancellationToken);

            ClearProjectsCache(id);
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
