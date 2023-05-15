using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using FluentNHibernate.Conventions.AcceptanceCriteria;

namespace WebCarteiraMvc
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = "SEUEMAIL";
            var pw = "SUASENHA";

            var client = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw)
            };

            MailMessage mailMessage = new MailMessage(from: mail, to: email, subject, message);
            mailMessage.IsBodyHtml = true;
           

            return client.SendMailAsync(mailMessage);
        }
    }
}
