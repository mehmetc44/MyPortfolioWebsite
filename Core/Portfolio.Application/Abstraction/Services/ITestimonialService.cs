using System;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Abstraction.Services;

public interface ITestimonialService
{
    Task<List<Testimonial>> GetAllTestimonialsAsync();
    Task AddTestimonialAsync(Testimonial testimonial);
    Task DeleteTestimonialAsync(Guid id);
    // Referansı sitede gizle/göster (Aktif/Pasif)
    Task ToggleStatusAsync(Guid id); 
}