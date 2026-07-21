using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Server.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Server.CQRS.Projects
{
    public record ReorderProjectsCommand(List<ProjectReorderItem> Items) : IRequest;

    public class ProjectReorderItem
    {
        public string Id { get; set; } = "";
        public int OrderIndex { get; set; }
    }

    public class ReorderProjectsCommandHandler : IRequestHandler<ReorderProjectsCommand>
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public ReorderProjectsCommandHandler(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task Handle(ReorderProjectsCommand request, CancellationToken cancellationToken)
        {
            if (request.Items == null || request.Items.Count == 0)
                return;

            foreach (var item in request.Items)
            {
                var project = await _context.Projects.FindAsync(new object[] { item.Id }, cancellationToken);
                if (project != null)
                {
                    project.OrderIndex = item.OrderIndex;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Clear projects lists caches
            _cache.Remove("Projects_tr");
            _cache.Remove("Projects_en");
            _cache.Remove("Projects_de");
        }
    }
}
