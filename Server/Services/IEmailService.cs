using System.Threading.Tasks;

namespace Server.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string subject, string body);
    }
}
