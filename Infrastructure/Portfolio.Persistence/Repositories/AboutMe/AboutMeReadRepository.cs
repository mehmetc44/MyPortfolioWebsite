using System;
using Portfolio.Persistence.Repositories;
using Portfolio.Application.Repositories.AboutMe;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.AboutMe;

public class AboutMeReadRepository : ReadRepository<Domain.Entities.AboutMe>, IAboutMeReadRepository
{
    public AboutMeReadRepository(PortfolioDbContext context) : base(context)
    {
    }
}
