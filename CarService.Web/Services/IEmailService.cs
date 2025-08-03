using System.Threading.Tasks;

namespace CarService.Web.Services
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string htmlBody);
    }
}
