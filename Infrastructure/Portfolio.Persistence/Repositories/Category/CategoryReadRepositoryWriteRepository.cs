using System;
using Portfolio.Application.Repositories.Category;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.Category;

public class CategoryWriteRepository : WriteRepository<Domain.Entities.Category>, ICategoryWriteRepository
{
    public CategoryWriteRepository(PortfolioDbContext context) : base(context)
    {
    }
}
