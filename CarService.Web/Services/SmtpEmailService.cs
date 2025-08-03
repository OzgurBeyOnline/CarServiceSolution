using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CarService.Web.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public SmtpEmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            var s = _config.GetSection("Smtp");
            var host = s["Host"];
            var port = int.Parse(s["Port"]!);
            var user = s["User"];
            var pass = s["Pass"];
            var from = s["From"];
            var disp = s["DisplayName"];

            using var msg = new MailMessage();
            msg.From = new MailAddress(from!, disp);
            msg.To.Add(to);
            msg.Subject = subject;
            msg.Body = htmlBody;
            msg.IsBodyHtml = true;

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(user, pass),
                EnableSsl = bool.Parse(s["EnableSsl"]!)
            };

            await client.SendMailAsync(msg);
        }
    }
}
