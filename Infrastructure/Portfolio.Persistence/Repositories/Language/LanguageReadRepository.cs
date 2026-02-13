using System;
using Portfolio.Persistence.Repositories;
using Portfolio.Application.Repositories.Language;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.Language;

public class LanguageReadRepository : ReadRepository<Domain.Entities.Language>, ILanguageReadRepository
{
    public LanguageReadRepository(PortfolioDbContext context) : base(context)
    {
    }
}
