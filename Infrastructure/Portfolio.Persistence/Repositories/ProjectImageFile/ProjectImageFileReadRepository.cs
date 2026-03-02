using Portfolio.Application.Repositories.ProjectImageFile;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.ProjectImageFile;

public class ProjectImageFileReadRepository : ReadRepository<Domain.Entities.ProjectImageFile>, IProjectImageFileReadRepository
{
    public ProjectImageFileReadRepository(PortfolioDbContext context) : base(context)
    {
    }
}
