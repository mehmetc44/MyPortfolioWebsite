using Portfolio.Application.Repositories.SiteImageFile;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.SiteImageFile;

public class SiteImageFileReadRepository : ReadRepository<Domain.Entities.SiteImageFile>, ISiteImageFileReadRepository
{
    public SiteImageFileReadRepository(PortfolioDbContext context) : base(context)
    {
    }
}
