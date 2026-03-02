using Portfolio.Application.Repositories.SiteImageFile;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.SiteImageFile;

public class SiteImageFileWriteRepository : WriteRepository<Domain.Entities.SiteImageFile>, ISiteImageFileWriteRepository
{
    public SiteImageFileWriteRepository(PortfolioDbContext context) : base(context)
    {
    }
}
