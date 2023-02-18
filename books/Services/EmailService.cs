using System.Net.Mail;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace books.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task SendEmailAsync(string from, string to, string subject, string html)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Site Administration", from));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart(TextFormat.Html) { Text = html };

        SmtpHiddenInfo smtpHiddenInfo = new SmtpHiddenInfo();
        _configuration.GetSection("SmtpHiddenInfo").Bind(smtpHiddenInfo);

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            smtpHiddenInfo.Host,
            smtpHiddenInfo.Port,
            (SecureSocketOptions)smtpHiddenInfo.SecureSocketOptions);


        await smtp.AuthenticateAsync(
            smtpHiddenInfo.User,
            smtpHiddenInfo.Password);

        await smtp.SendAsync(email);

        await smtp.DisconnectAsync(true);
    }

}