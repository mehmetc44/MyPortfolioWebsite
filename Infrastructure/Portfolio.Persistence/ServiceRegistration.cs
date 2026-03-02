using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Application.Repositories.AboutMe;
using Portfolio.Application.Repositories.Category;
using Portfolio.Application.Repositories.ContactMessage;
using Portfolio.Application.Repositories.File;
using Portfolio.Application.Repositories.Language;
using Portfolio.Application.Repositories.Project;
using Portfolio.Application.Repositories.ProjectImageFile;
using Portfolio.Application.Repositories.ResumeFile;
using Portfolio.Application.Repositories.SiteImageFile;
using Portfolio.Application.Repositories.Skill;
using Portfolio.Application.Repositories.Testimonial;
using Portfolio.Application.Repositories.Timeline;
using Portfolio.Domain.Entities.Identity;
using Portfolio.Persistence.Context;
using Portfolio.Persistence.Repositories.AboutMe;
using Portfolio.Persistence.Repositories.Category;
using Portfolio.Persistence.Repositories.ContactMessage;
using Portfolio.Persistence.Repositories.File;
using Portfolio.Persistence.Repositories.Language;
using Portfolio.Persistence.Repositories.Project;
using Portfolio.Persistence.Repositories.ProjectImageFile;
using Portfolio.Persistence.Repositories.ResumeFile;
using Portfolio.Persistence.Repositories.SiteImageFile;
using Portfolio.Persistence.Repositories.Skill;
using Portfolio.Persistence.Repositories.Testimonial;
using Portfolio.Persistence.Repositories.Timeline;


namespace Portfolio.Persistence;

public static class ServiceRegistration
{
    public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PortfolioDbContext>(options =>
    options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));
        services.AddIdentity<AspUser, AspRole>()
        .AddEntityFrameworkStores<PortfolioDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IAboutMeReadRepository, AboutMeReadRepository>();
        services.AddScoped<IAboutMeWriteRepository, AboutMeWriteRepository>();

        services.AddScoped<ICategoryReadRepository, CategoryReadRepository>();
        services.AddScoped<ICategoryWriteRepository, CategoryWriteRepository>();

        services.AddScoped<IContactMessageReadRepository, ContactMessageReadRepository>();
        services.AddScoped<IContactMessageWriteRepository, ContactMessageWriteRepository>();

        services.AddScoped<ILanguageReadRepository, LanguageReadRepository>();
        services.AddScoped<ILanguageWriteRepository, LanguageWriteRepository>();


        services.AddScoped<IProjectReadRepository, ProjectReadRepository>();
        services.AddScoped<IProjectWriteRepository, ProjectWriteRepository>();

        services.AddScoped<ISkillReadRepository, SkillReadRepository>();
        services.AddScoped<ISkillWriteRepository, SkillWriteRepository>();

        services.AddScoped<ITestimonialReadRepository, TestimonialReadRepository>();
        services.AddScoped<ITestimonialWriteRepository, TestimonialWriteRepository>();

        services.AddScoped<ITimelineReadRepository, TimelineReadRepository>();
        services.AddScoped<ITimelineWriteRepository, TimelineWriteRepository>();
        
        services.AddScoped<IFileReadRepository, FileReadRepository>();
        services.AddScoped<IFileWriteRepository, FileWriteRepository>();

        services.AddScoped<IResumeFileReadRepository, ResumeFileReadRepository>();
        services.AddScoped<IResumeFileWriteRepository, ResumeFileWriteRepository>();

        services.AddScoped<ISiteImageFileReadRepository, SiteImageFileReadRepository>();
        services.AddScoped<ISiteImageFileWriteRepository, SiteImageFileWriteRepository>();

        services.AddScoped<IProjectImageFileReadRepository, ProjectImageFileReadRepository>();
        services.AddScoped<IProjectImageFileWriteRepository, ProjectImageFileWriteRepository>();

    }
}
