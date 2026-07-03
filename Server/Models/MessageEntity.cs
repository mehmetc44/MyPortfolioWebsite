using System;

namespace Server.Models
{
    public class MessageEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Message { get; set; } = "";
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }
}
