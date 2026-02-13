using System;
using Portfolio.Application.Repositories.AboutMe;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.AboutMe;

public class AboutMeWriteRepository : WriteRepository<Domain.Entities.AboutMe>, IAboutMeWriteRepository
{
    public AboutMeWriteRepository(PortfolioDbContext context) : base(context)
    {
    }
}
