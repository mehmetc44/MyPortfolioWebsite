using System;
using Portfolio.Application.Repositories.Timeline;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.Timeline;

public class TimelineWriteRepository : WriteRepository<Domain.Entities.Timeline>, ITimelineWriteRepository
{
    public TimelineWriteRepository(PortfolioDbContext context) : base(context)
    {
    }
}
