using Microsoft.EntityFrameworkCore;
using UniversityB.MockApi.Data;
using UniversityB.MockApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Ensure the Db folder exists so SQLite database creation does not fail
var dbFolder = Path.Combine(builder.Environment.ContentRootPath, "Db");
if (!Directory.Exists(dbFolder))
{
    Directory.CreateDirectory(dbFolder);
}

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register UniversityDbContext with SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<UniversityDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();

// Auto-migrate (EnsureCreated) and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UniversityDbContext>();
    db.Database.EnsureCreated();

    if (!db.Students.Any())
    {
        db.Students.AddRange(
            new Student { FirstName = "Can", LastName = "Demir", StudentNumber = "B2021051", Email = "can.demir@uni-b.edu", Department = "Makine Mühendisliği", Gpa = 3.12 },
            new Student { FirstName = "Merve", LastName = "Kaya", StudentNumber = "B2021052", Email = "merve.kaya@uni-b.edu", Department = "Endüstri Mühendisliği", Gpa = 3.75 },
            new Student { FirstName = "Bora", LastName = "Şahin", StudentNumber = "B2022053", Email = "bora.sahin@uni-b.edu", Department = "Elektrik-Elektronik Mühendisliği", Gpa = 2.84 }
        );
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Basic API endpoint to get all students (verification of EF Core + SQLite setup)
app.MapGet("/api/students", async (UniversityDbContext db) =>
{
    return await db.Students.ToListAsync();
})
.WithName("GetStudents")
.WithOpenApi();

app.Run();
