using System;
using Portfolio.Persistence.Repositories;
using Portfolio.Application.Repositories.PersonalInfo;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.PersonalInfo;

public class PersonalInfoReadRepository : ReadRepository<Domain.Entities.PersonalInfo>, IPersonalInfoReadRepository
{
    public PersonalInfoReadRepository(PortfolioDbContext context) : base(context)
    {
    }
}
