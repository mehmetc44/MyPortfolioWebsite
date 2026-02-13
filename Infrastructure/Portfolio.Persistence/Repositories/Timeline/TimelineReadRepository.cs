using System;
using Portfolio.Application.Repositories.Timeline;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.Timeline;

public class TimelineReadRepository : ReadRepository<Domain.Entities.Timeline>, ITimelineReadRepository
{
    public TimelineReadRepository(PortfolioDbContext context) : base(context)
    {
    }
}
