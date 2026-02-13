using System;
using Portfolio.Application.Repositories.Language;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.Language;

public class LanguageWriteRepository : WriteRepository<Domain.Entities.Language>, ILanguageWriteRepository
{
    public LanguageWriteRepository(PortfolioDbContext context) : base(context)
    {
    }
}
