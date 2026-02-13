using System;
using Portfolio.Application.Repositories.Testimonial;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.Testimonial;

public class TestimonialReadRepository : ReadRepository<Domain.Entities.Testimonial>, ITestimonialReadRepository
{
    public TestimonialReadRepository(PortfolioDbContext context) : base(context)
    {
    }
}
