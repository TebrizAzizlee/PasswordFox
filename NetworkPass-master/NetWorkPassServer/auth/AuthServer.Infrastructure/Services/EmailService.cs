using AuthServer.Application.Abstractions;
using AuthServer.Application.Services;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

using MailKit.Net.Smtp;

namespace AuthServer.Infrastructure.Services;
public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
        if (string.IsNullOrWhiteSpace(_settings.FromEmail))
            throw new InvalidOperationException("EmailSettings:FromEmail boş ola bilməz.");
    }
    public async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("Support", _settings.FromEmail));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        email.Body = new TextPart("html") { Text = body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls,cancellationToken);
        await smtp.AuthenticateAsync(_settings.Username, _settings.Password,cancellationToken);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true,cancellationToken);
    }
}
