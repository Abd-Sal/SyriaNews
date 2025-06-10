namespace SyriaNews.HelperTools;

public class EmailSendingHelp
{
    [AutomaticRetry(Attempts = 5, DelaysInSeconds = new[] { 60, 120, 300 })]
    public static async Task<bool> SendEmailAsync(
        string fullName,
        ApplicationUser user,
        string code,
        string origin,
        MailSettings mailSettings
        )
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
        emailMessage.To.Add(MailboxAddress.Parse(user.Email!));
        emailMessage.Subject = "✅ Syria News: Email Confirmation";

        var confirmationLink = $"{origin}/auth/confirm-email?UserId={user.Id}&Code={code}";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $"<p>Please confirm your email by clicking the link: <a href='{confirmationLink}'>Confirm Email</a></p>"
        };
        emailMessage.Body = bodyBuilder.ToMessageBody();
        try
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(mailSettings.Mail, mailSettings.Password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
                return true;
            }
        }
        catch
        {
            return false;
        }
    }

    [AutomaticRetry(Attempts = 5, DelaysInSeconds = new[] { 60, 120, 300 })]
    public static async Task<bool> SendResetPasswordEmailAsync(
       string fullName,
       ApplicationUser user,
       string code,
       string origin,
       MailSettings mailSettings)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
        emailMessage.To.Add(MailboxAddress.Parse(user.Email!));
        emailMessage.Subject = "✅ Syria News: Change Password";

        var confirmationLink = $"{origin}/auth/forget-password?Email={user.Email!}";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $"Please reset your email by clicking the link: {confirmationLink}"
        };
        emailMessage.Body = bodyBuilder.ToMessageBody();
        try
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(mailSettings.Host, mailSettings.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(mailSettings.Mail, mailSettings.Password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
                return true;
            }
        }
        catch
        {
            return false;
        }
    }

}
