using Portfolio.Application.Repositories.File;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.File;

public class FileReadRepository : ReadRepository<Domain.Entities.File>, IFileReadRepository
{
    public FileReadRepository(PortfolioDbContext context) : base(context)
    {
    }
}
