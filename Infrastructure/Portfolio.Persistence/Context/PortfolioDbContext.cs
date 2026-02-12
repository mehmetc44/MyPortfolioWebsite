using System;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Entities.Identity;

namespace Portfolio.Persistence.Context;

public class PortfolioDbContext : IdentityDbContext<AspUser, IdentityRole<Guid>, Guid>
    {
        public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : base(options)
        {
        }

        // --- TABLOLARIMIZ (DbSet) ---
        
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectImage> ProjectImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Language> Languages { get; set; } // Düz string (MultiLanguage değil)
        public DbSet<Timeline> Timelines { get; set; }
        public DbSet<Testimonial> Testimonials { get; set; }
        public DbSet<AboutMe> AboutMe { get; set; } // Tekil tablo (Admin paneli ayarları)
        public DbSet<ContactMessage> ContactMessages { get; set; }

        // Not: AppUser ve AppRole tabloları (AspNetUsers vb.) IdentityDbContext tarafından otomatik gelir.

        // --- AYARLAR (Fluent API) ---
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. ÖNCE IDENTITY AYARLARINI YÜKLE (Çok Önemli!)
            // Bunu yazmazsan "IdentityUserLogin<Guid>" hatası alırsın.
            base.OnModelCreating(modelBuilder);

            // 2. DIŞARIDAN GELEN CONFIGURATION DOSYALARINI YÜKLE
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // 3. MULTI-LANGUAGE (Value Object) AYARLARI
            // Bu alanların ayrı tablo olmamasını, ana tabloya gömülmesini sağlar.

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
            modelBuilder.Entity<AboutMe>().OwnsOne(a => a.Introduction);
            modelBuilder.Entity<AboutMe>().OwnsOne(a => a.Biography);
            modelBuilder.Entity<AboutMe>().OwnsOne(a => a.CvPath); // CV Dosya yolları

            // Not: Language entity'si için OwnsOne YAPMIYORUZ, çünkü düz string yaptık.

            // 4. SQLITE GUID DÜZELTMESİ
            // SQLite, GUID'leri varsayılan olarak BLOB (Binary) tutar.
            // Okunabilir olması için TEXT'e çeviriyoruz.
            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    var properties = entityType.ClrType.GetProperties()
                        .Where(p => p.PropertyType == typeof(Guid) && p.Name != "Id" && p.Name != "ProjectId" && p.Name != "CategoryId");
                }
            }
        }
    }
