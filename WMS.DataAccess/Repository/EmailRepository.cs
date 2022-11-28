using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using WMS.DataAccess.Repository.IRepository;
using WMS.Models;
using WMS.Models.ViewModels;

namespace WMS.DataAccess.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly IConfiguration _configuration;

        public EmailRepository(IConfiguration config)
        {
            _configuration = config;
        }

        public void SendEmail(EmailViewModel request)
        {
            var objEmail = _configuration.GetSection("Email").Get<Email>();

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(objEmail.Username));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = request.Body };

            using var smtp = new SmtpClient();
            smtp.Connect(objEmail.Host, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(objEmail.Username, objEmail.Password);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
