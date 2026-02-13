using System;
using Portfolio.Application.Repositories.Testimonial;
using Portfolio.Persistence.Context;

namespace Portfolio.Persistence.Repositories.Testimonial;

public class TestimonialWriteRepository : WriteRepository<Domain.Entities.Testimonial>, ITestimonialWriteRepository
{
    public TestimonialWriteRepository(PortfolioDbContext context) : base(context)
    {
    }
}
