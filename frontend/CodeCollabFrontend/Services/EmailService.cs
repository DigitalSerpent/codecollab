using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CodeCollabFrontend.Services;

public class EmailService
{
    private readonly string _smtpHost = "smtp.yandex.ru";
    private readonly int _smtpPort = 465;
    private readonly string _emailFrom = "code.collab@yandex.ru";
    private readonly string _password = "amxhgjaezfzqhhpc";

    public async Task SendCodeAsync(string toEmail, string code)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Code Collab", _emailFrom));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "Код подтверждения Code Collab";

        var body = $@"
            <div style='font-family: Arial, sans-serif; max-width: 500px; margin: 0 auto; padding: 20px; border: 1px solid #00aaff; border-radius: 16px; background: #121628; color: #e0e0e0;'>
                <h2 style='color: #00aaff; text-align: center;'>Code Collab</h2>
                <p style='font-size: 16px;'>Ваш код подтверждения:</p>
                <div style='font-size: 48px; font-weight: bold; text-align: center; letter-spacing: 8px; margin: 20px 0; color: #00aaff;'>
                    {code}
                </div>
                <p style='font-size: 14px; color: #a0a0a0;'>Если вы не запрашивали код — просто проигнорируйте это письмо.</p>
                <hr style='border-color: #2a2a4a;'>
                <p style='font-size: 12px; color: #a0a0a0; text-align: center;'>Не нужно отвечать на это сообщение.</p>
            </div>";

        message.Body = new TextPart("html") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.SslOnConnect);
        await client.AuthenticateAsync(_emailFrom, _password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}