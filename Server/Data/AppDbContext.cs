using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ProfileEntity> Profiles { get; set; }
        public DbSet<ProjectEntity> Projects { get; set; }
        public DbSet<ArticleEntity> Articles { get; set; }
        public DbSet<MessageEntity> Messages { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<SkillEntity> Skills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure primary keys
            modelBuilder.Entity<ProfileEntity>().HasKey(p => p.Id);
            modelBuilder.Entity<ProjectEntity>().HasKey(p => p.Id);
            modelBuilder.Entity<ArticleEntity>().HasKey(a => a.Id);
            modelBuilder.Entity<MessageEntity>().HasKey(m => m.Id);
            modelBuilder.Entity<UserEntity>().HasKey(u => u.Id);
            modelBuilder.Entity<SkillEntity>().HasKey(s => s.Id);
        }
    }
}
