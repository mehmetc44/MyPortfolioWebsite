using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Portfolio.Persistence.Context;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PortfolioDbContext>
{
    public PortfolioDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<PortfolioDbContext>();
        var connectionString = "Data Source=Portfolio.db";
        builder.UseSqlite(connectionString);
        return new PortfolioDbContext(builder.Options);
    }
}
