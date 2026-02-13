using System;
using Portfolio.Persistence.Repositories;
using Portfolio.Application.Repositories.Category;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.Category;

public class CategoryReadRepository : ReadRepository<Domain.Entities.Category>, ICategoryReadRepository
{
    public CategoryReadRepository(PortfolioDbContext context) : base(context)
    {
    }
}
