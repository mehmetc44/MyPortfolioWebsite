using Microsoft.EntityFrameworkCore;
using UniversityA.MockApi.Data;
using UniversityA.MockApi.Models;

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
            new Student { FirstName = "Ali", LastName = "Yılmaz", StudentNumber = "A2021001", Email = "ali.yilmaz@uni-a.edu", Department = "Bilgisayar Mühendisliği", Gpa = 3.45 },
            new Student { FirstName = "Zeynep", LastName = "Çelik", StudentNumber = "A2021002", Email = "zeynep.celik@uni-a.edu", Department = "Yazılım Mühendisliği", Gpa = 3.82 },
            new Student { FirstName = "Kaan", LastName = "Öztürk", StudentNumber = "A2022003", Email = "kaan.ozturk@uni-a.edu", Department = "Yapay Zeka Mühendisliği", Gpa = 2.91 }
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
