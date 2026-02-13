using System;
using Portfolio.Persistence.Repositories;
using Portfolio.Application.Repositories.ContactMessage;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.ContactMessage;

public class ContactMessageReadRepository : ReadRepository<Domain.Entities.ContactMessage>, IContactMessageReadRepository
{
    public ContactMessageReadRepository(PortfolioDbContext context) : base(context)
    {
    }
}
