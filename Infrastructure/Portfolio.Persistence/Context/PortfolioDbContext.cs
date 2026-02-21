using System;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Entities.Identity;

namespace Portfolio.Persistence.Context;

public class PortfolioDbContext : IdentityDbContext<AspUser, AspRole, Guid>
{
    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : base(options)
    {
    }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectImage> ProjectImages { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Timeline> Timelines { get; set; }
    public DbSet<Testimonial> Testimonials { get; set; }
    public DbSet<AboutMe> AboutMe { get; set; }
    public DbSet<ContactMessage> ContactMessages { get; set; }

    // Not: AppUser ve AppRole tabloları (AspNetUsers vb.) IdentityDbContext tarafından otomatik gelir.

    // --- AYARLAR (Fluent API) ---
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. IDENTITY AYARLARI
        base.OnModelCreating(modelBuilder);

        // 2. CONFIGURATION DOSYALARI
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // 3. MULTI-LANGUAGE (Value Object) AYARLARI

        // Project
        modelBuilder.Entity<Project>().OwnsOne(p => p.Title);
        modelBuilder.Entity<Project>().OwnsOne(p => p.Description);

        // Category
        modelBuilder.Entity<Category>().OwnsOne(c => c.Name);

        // Skill
        modelBuilder.Entity<Skill>().OwnsOne(s => s.Title);

        // Timeline
        modelBuilder.Entity<Timeline>().OwnsOne(t => t.Title);
        modelBuilder.Entity<Timeline>().OwnsOne(t => t.CompanyOrSchool);
        modelBuilder.Entity<Timeline>().OwnsOne(t => t.Description);

        // Testimonial
        modelBuilder.Entity<Testimonial>().OwnsOne(t => t.Comment);

        // AboutMe
        modelBuilder.Entity<AboutMe>().OwnsOne(a=>a.Title);
        modelBuilder.Entity<AboutMe>().OwnsOne(a => a.Biography);

        // --- EKSİK OLAN KISIM BURASIYDI ---
        // Language entity'sindeki Name alanı MultiLanguageString olduğu için bunu eklemek ZORUNDASIN.
        modelBuilder.Entity<Language>().OwnsOne(l => l.Name);


        // 4. SQLITE GUID DÜZELTMESİ (Kodun yarım kalmıştı, tamamladım)
        if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.ClrType.GetProperties()
                    .Where(p => p.PropertyType == typeof(Guid) && p.Name != "Id" && p.Name != "ProjectId" && p.Name != "CategoryId");

                // Bu döngünün içi boştu, işlevsizdi. Şöyle olmalı:
                foreach (var property in properties)
                {
                    modelBuilder.Entity(entityType.Name).Property(property.Name).HasConversion<string>();
                }
            }
        }
    }
}
