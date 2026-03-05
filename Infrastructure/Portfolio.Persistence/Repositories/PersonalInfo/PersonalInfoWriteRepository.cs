using System;
using Portfolio.Application.Repositories.PersonalInfo;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.PersonalInfo;

public class PersonalInfoWriteRepository : WriteRepository<Domain.Entities.PersonalInfo>, IPersonalInfoWriteRepository
{
    public PersonalInfoWriteRepository(PortfolioDbContext context) : base(context)
    {
    }
}
