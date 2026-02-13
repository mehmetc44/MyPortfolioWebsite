using System;
using Portfolio.Application.Repositories.Skill;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.Skill;

public class SkillWriteRepository : WriteRepository<Domain.Entities.Skill>, ISkillWriteRepository
{
    public SkillWriteRepository(PortfolioDbContext context) : base(context)
    {
    }
}
