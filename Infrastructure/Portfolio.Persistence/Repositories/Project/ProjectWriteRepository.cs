using System;
using Portfolio.Application.Repositories.Project;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.Project;

public class ProjectWriteRepository : WriteRepository<Domain.Entities.Project>, IProjectWriteRepository
{
    public ProjectWriteRepository(PortfolioDbContext context) : base(context)
    {
    }
}
