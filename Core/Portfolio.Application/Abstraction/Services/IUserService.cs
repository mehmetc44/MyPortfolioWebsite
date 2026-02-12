using System;
using Portfolio.Domain.Entities.Identity;

namespace Portfolio.Application.Abstraction.Services;

public interface IUserService
{
    // Şu anki admini getir
    Task<AspUser> GetCurrentUserAsync(string username);
    
    // Profil bilgilerini güncelle (Ad, Soyad, Email, Avatar)
    Task UpdateUserProfileAsync(AspUser user, string? newPassword);
    
    // Sadece şifre kontrolü (Eski şifre doğru mu?)
    Task<bool> CheckPasswordAsync(AspUser user, string password);
}
