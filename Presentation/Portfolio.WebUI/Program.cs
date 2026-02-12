using Portfolio.WebUI.Services;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Portfolio.WebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            builder.Services.AddHttpContextAccessor();
            // DbContext servisi hemen üstte olmalı...
            builder.Services.AddDbContext<DbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


            // HEMEN ALTINA BUNU EKLE:
            builder.Services.AddIdentity<AspUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<DbContext>()
                .AddDefaultTokenProviders();

            // Şifre Kuralları (Geliştirme aşamasında seni yormaması için basit yapalım)
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 3; // Şifre: "123" olabilsin diye :)
            });

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