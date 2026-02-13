using System;
using Portfolio.Application.Repositories.ContactMessage;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.ContactMessage;

public class ContactMessageWriteRepository : WriteRepository<Domain.Entities.ContactMessage>, IContactMessageWriteRepository
{
    public ContactMessageWriteRepository(PortfolioDbContext context) : base(context)
    {
    }
}
