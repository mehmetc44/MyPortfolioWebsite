using System;

namespace Portfolio.Domain.Entities.Identity;

public class AspUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        
        // Admin panel sağ üst köşede görünecek küçük profil fotosu
        // (AboutMe'deki büyük fotoğraftan farklı olabilir)
        public string? ImageUrl { get; set; } 
    }