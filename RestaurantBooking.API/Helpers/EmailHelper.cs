using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using RestaurantBooking.API.Models.DTO;

namespace RestaurantBooking.API.Helpers
{
    public static class EmailHelper
    {
        public async static Task SendEmailAsync(EmailDto request, IConfiguration configuration)
        {
            string host = configuration["MailSettings:host"]!;
            int port = int.Parse(configuration["MailSettings:port"]!);
            string user = configuration["MailSettings:auth:user"]!;
            string password = configuration["MailSettings:auth:pass"]!;

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(user));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = $"<p>{request.Body}</p>" };

            var smtp = new SmtpClient();
            await smtp.ConnectAsync(host, port);
            await smtp.AuthenticateAsync(user, password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
