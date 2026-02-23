using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Portfolio.Application.Mapper;
using Portfolio.WebUI.Extensions;
using Portfolio.Persistence;
using Portfolio.Infrastructure;
using Portfolio.Infrastructure.Enums;

namespace Portfolio.WebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddPersistenceServices(builder.Configuration);
            builder.Services.AddInfrastructureServices();
            builder.Services.AddStorage(StorageType.Local);
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddAutoMapper(typeof(AutoMappingProfile).Assembly);
            builder.Services.ConfigureLocalization();
            builder.Services.AddControllersWithViews()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    factory.Create(typeof(SharedResource));
                });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("tr"), 
                SupportedCultures = new[]
            {
                new CultureInfo("tr"),
                new CultureInfo("en"),
                new CultureInfo("de")
            },          
                SupportedUICultures = new[]
            {
                new CultureInfo("tr"),
                new CultureInfo("en"),
                new CultureInfo("de")
            }         
            });


            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}