using System;
using Portfolio.Application.Repositories.ResumeFile;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.ResumeFile;

public class ResumeFileWriteRepository : WriteRepository<Domain.Entities.ResumeFile>, IResumeFileWriteRepository
{
    public ResumeFileWriteRepository(PortfolioDbContext context) : base(context)
    {
    }
}
