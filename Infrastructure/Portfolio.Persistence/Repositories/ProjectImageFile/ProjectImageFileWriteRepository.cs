using System;
using Portfolio.Application.Repositories.ProjectImageFile;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.ProjectImageFile;

public class ProjectImageFileWriteRepository : WriteRepository<Domain.Entities.ProjectImageFile>, IProjectImageFileWriteRepository
{
    public ProjectImageFileWriteRepository(PortfolioDbContext context) : base(context)
    {
    }
}
