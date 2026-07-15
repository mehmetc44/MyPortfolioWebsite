using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Services
{
    public interface IDatabaseSeeder
    {
        void Seed();
    }

    public class DatabaseSeeder : IDatabaseSeeder
    {
        private readonly AppDbContext _db;

        public DatabaseSeeder(AppDbContext db)
        {
            _db = db;
        }

        public void Seed()
        {
            if (_db.Database.ProviderName == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                try
                {
                    _db.Database.ExecuteSqlRaw(@"
                        CREATE TABLE IF NOT EXISTS ""__EFMigrationsHistory"" (
                            ""MigrationId"" character varying(150) NOT NULL,
                            ""ProductVersion"" character varying(32) NOT NULL,
                            CONSTRAINT ""PK___EFMigrationsHistory"" PRIMARY KEY (""MigrationId"")
                        );
                        
                        ALTER TABLE ""Profiles"" ADD COLUMN IF NOT EXISTS ""CvText_TR"" text;
                        ALTER TABLE ""Profiles"" ADD COLUMN IF NOT EXISTS ""CvText_EN"" text;
                        ALTER TABLE ""Profiles"" ADD COLUMN IF NOT EXISTS ""CvText_DE"" text;
                        ALTER TABLE ""Profiles"" ADD COLUMN IF NOT EXISTS ""CvPdfUrl_TR"" text;
                        ALTER TABLE ""Profiles"" ADD COLUMN IF NOT EXISTS ""CvPdfUrl_EN"" text;
                        ALTER TABLE ""Profiles"" ADD COLUMN IF NOT EXISTS ""CvPdfUrl_DE"" text;

                        INSERT INTO ""__EFMigrationsHistory"" (""MigrationId"", ""ProductVersion"")
                        VALUES ('20260714132915_InitialCreate', '8.0.8')
                        ON CONFLICT (""MigrationId"") DO NOTHING;
                    ");
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"[Schema Check Warning] {ex.Message}");
                }
            }

            _db.Database.Migrate();

            SeedUsers();

            _db.SaveChanges();
        }

        private void SeedUsers()
        {
            if (!_db.Users.Any())
            {
                _db.Users.Add(new UserEntity
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = PasswordHasher.HashPassword("admin123")
                });
            }
        }
    }
}
