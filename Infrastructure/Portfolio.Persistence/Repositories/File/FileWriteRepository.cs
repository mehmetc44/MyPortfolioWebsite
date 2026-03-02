using System;
using Portfolio.Application.Repositories.File;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.File;

public class FileWriteRepository : WriteRepository<Domain.Entities.File>, IFileWriteRepository
{
    public FileWriteRepository(PortfolioDbContext context) : base(context)
    {
    }
}
