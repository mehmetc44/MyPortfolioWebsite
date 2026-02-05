using Portfolio.WebUI.Services;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.Extensions.Options;

namespace Portfolio.WebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            builder.Services.AddHttpContextAccessor();

            var supportedCultures = new[]
            {
                new CultureInfo("en-US"), 
                new CultureInfo("tr-TR")
            };

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("tr-TR");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider()); 
            });

            builder.Services.AddControllersWithViews()
                .AddViewLocalization(); 

            builder.Services.AddSingleton<ContentService>(); 
            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            var localizationOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(localizationOptions.Value);
           

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