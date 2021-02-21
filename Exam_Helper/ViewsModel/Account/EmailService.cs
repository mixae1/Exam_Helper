using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using MailKit;
namespace Exam_Helper.Services
{
    public class EmailService
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация сайта", "ex.helper@yandex.ru"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                //client.ServerCertificateValidationCallback =
                //    (sender, sertificate, certChainType, errors) => true;
                //client.AuthenticationMechanisms.Remove("XOAUTH2");

                
                await client.ConnectAsync("smtp.yandex.com", 465, true);
                await client.AuthenticateAsync("ex.helper@yandex.ru", "nvpwtrsxyqxbshuq"); //
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
