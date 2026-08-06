using Microsoft.EntityFrameworkCore;
using UniversityB.MockApi.Models;

namespace UniversityB.MockApi.Data
{
    public class UniversityDbContext : DbContext
    {
        public UniversityDbContext(DbContextOptions<UniversityDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students => Set<Student>();
    }
}
