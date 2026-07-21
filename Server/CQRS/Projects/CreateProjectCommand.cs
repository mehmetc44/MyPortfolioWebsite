using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;
using Server.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Server.CQRS.Projects
{
    public record CreateProjectCommand(ProjectEntity Project) : IRequest<ProjectEntity>;

    public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectEntity>
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public CreateProjectCommandHandler(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<ProjectEntity> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var project = request.Project;
            if (string.IsNullOrWhiteSpace(project.Id))
            {
                throw new ArgumentException("Proje ID'si boş olamaz.");
            }

            var exists = await _context.Projects.AnyAsync(p => p.Id == project.Id, cancellationToken);
            if (exists)
            {
                throw new InvalidOperationException("Bu ID'ye sahip bir proje zaten mevcut.");
            }

            _context.Projects.Add(project);
            await _context.SaveChangesAsync(cancellationToken);

            ClearProjectsCache(project.Id);

            return project;
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
    }
}
