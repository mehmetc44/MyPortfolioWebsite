using System;
using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Abstraction.Services;
using Portfolio.Application.Repositories.PersonalInfo;
using Portfolio.Application.Repositories.Category;
using Portfolio.Application.Repositories.ContactMessage;
using Portfolio.Application.Repositories.Language;
using Portfolio.Application.Repositories.Project;
using Portfolio.Application.Repositories.Skill;
using Portfolio.Application.Repositories.Testimonial;
using Portfolio.Application.Repositories.Timeline;
using Portfolio.Domain.Entities.Identity;
using Portfolio.Infrastructure.Services;
using Portfolio.Persistence.Context;
using Portfolio.Persistence.Repositories.PersonalInfo;
using Portfolio.Persistence.Repositories.Category;
using Portfolio.Persistence.Repositories.ContactMessage;
using Portfolio.Persistence.Repositories.Language;
using Portfolio.Persistence.Repositories.Project;
using Portfolio.Persistence.Repositories.Skill;
using Portfolio.Persistence.Repositories.Testimonial;
using Portfolio.Persistence.Repositories.Timeline;

namespace Portfolio.WebUI.Extensions;

public static class ServiceRegistration
{
    public static void ConfigureLocalization(this IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = "Resources");
            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("tr-TR")
            };
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("tr-TR");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());
            });
    }


}
