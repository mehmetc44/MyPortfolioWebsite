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
    public DbSet<Domain.Entities.File> Files { get; set; }
    public DbSet<ProjectImageFile> ProjectImages { get; set; }
    public DbSet<ResumeFile> ResumeFiles { get; set; }
    public DbSet<SiteImageFile> SiteImageFiles { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Timeline> Timelines { get; set; }
    public DbSet<Testimonial> Testimonials { get; set; }
    public DbSet<PersonalInfo> AboutMe { get; set; }
    public DbSet<ContactMessage> ContactMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<Project>().OwnsOne(p => p.Title);
        modelBuilder.Entity<Project>().OwnsOne(p => p.Description);

        modelBuilder.Entity<Category>().OwnsOne(c => c.Name);

        modelBuilder.Entity<Skill>().OwnsOne(s => s.Title);

        modelBuilder.Entity<Timeline>().OwnsOne(t => t.Title);
        modelBuilder.Entity<Timeline>().OwnsOne(t => t.CompanyOrSchool);
        modelBuilder.Entity<Timeline>().OwnsOne(t => t.Description);

        modelBuilder.Entity<Testimonial>().OwnsOne(t => t.Comment);
        modelBuilder.Entity<PersonalInfo>().OwnsOne(a=>a.Title);
        modelBuilder.Entity<PersonalInfo>().OwnsOne(a => a.Biography);

        modelBuilder.Entity<Language>().OwnsOne(l => l.Name);

        if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
{
    foreach (var entityType in modelBuilder.Model.GetEntityTypes())
    {
        if (entityType.GetTableName().StartsWith("AspNet"))
            continue;

        var properties = entityType.ClrType.GetProperties()
            .Where(p => p.PropertyType == typeof(Guid)
                     && p.Name != "Id"
                     && p.Name != "ProjectId"
                     && p.Name != "CategoryId");

        foreach (var property in properties)
        {
            modelBuilder.Entity(entityType.ClrType)
                        .Property(property.Name)
                        .HasConversion<string>();
        }
    }
}
    }
}
