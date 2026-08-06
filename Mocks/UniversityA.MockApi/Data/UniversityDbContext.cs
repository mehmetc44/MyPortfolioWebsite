using Microsoft.EntityFrameworkCore;
using UniversityA.MockApi.Models;

namespace UniversityA.MockApi.Data
{
    public class UniversityDbContext : DbContext
    {
        public UniversityDbContext(DbContextOptions<UniversityDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students => Set<Student>();
    }
}
