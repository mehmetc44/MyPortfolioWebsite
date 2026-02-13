using System;
using Portfolio.Application.Repositories.Skill;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.Skill;

public class SkillReadRepository : ReadRepository<Domain.Entities.Skill>, ISkillReadRepository
{
    public SkillReadRepository(PortfolioDbContext context) : base(context)
    {
    }
}
