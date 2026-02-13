using System;
using Portfolio.Application.Repositories.Project;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.Project;

public class ProjectReadRepository : ReadRepository<Domain.Entities.Project>, IProjectReadRepository
{
    public ProjectReadRepository(PortfolioDbContext context) : base(context)
    {
    }
}
