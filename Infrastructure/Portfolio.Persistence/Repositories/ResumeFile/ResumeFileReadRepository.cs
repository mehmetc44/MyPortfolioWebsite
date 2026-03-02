using Portfolio.Application.Repositories.ResumeFile;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.ResumeFile;

public class ResumeFileReadRepository : ReadRepository<Domain.Entities.ResumeFile>, IResumeFileReadRepository
{
    public ResumeFileReadRepository(PortfolioDbContext context) : base(context)
    {
    }
}
