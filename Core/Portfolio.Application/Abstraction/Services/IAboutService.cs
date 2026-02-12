using System;

namespace Portfolio.Application.Abstraction.Services;

using Portfolio.Domain.Entities;

public interface IAboutService
{
    // Tek bir kayıt olduğu için ID istemiyoruz
    Task<AboutMe> GetAboutMeAsync();
    
    // Güncelleme (Yoksa oluşturma mantığı içerde olacak)
    Task UpdateAboutMeAsync(AboutMe aboutMe);
}
