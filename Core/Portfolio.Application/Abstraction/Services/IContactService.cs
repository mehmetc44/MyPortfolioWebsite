using System;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Abstraction.Services;

public interface IContactService
{
    // 1. Ziyaretçi: Mesaj Gönderir
    Task SendMessageAsync(ContactMessage message);

    // 2. Admin (Sen): Mesajları Listeler
    Task<List<ContactMessage>> GetAllMessagesAsync();
    
    // 3. Admin: Okunmamış mesaj sayısını getir (Dashboard'daki bildirim için)
    Task<int> GetUnreadCountAsync();

    // 4. Admin: Mesajı "Okundu" olarak işaretle (Detayına bakınca)
    Task MarkAsReadAsync(Guid id);

    // 5. Admin: Gereksiz mesajı sil
    Task DeleteMessageAsync(Guid id);
}
