using System;
using Microsoft.AspNetCore.Identity;


namespace Portfolio.Domain.Entities.Identity;

public class AspUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; } 
}