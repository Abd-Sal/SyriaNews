namespace SyriaNews.Repository.Implementations;

public class NotificationSender(IOptions<EmailConfigurationsOptions> emailOptions, ILogger<NotificationSender> logger) : INotificationSender
{
    private readonly EmailConfigurationsOptions _emailOptions = emailOptions.Value;
    private readonly ILogger<NotificationSender> logger = logger;

    public Result SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            BackgroundJob.Enqueue(() => SendEmailInBackground(toEmail, subject, body));
            logger.LogInformation("Eagle system send email for:({email})", toEmail);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(new Error("Error.UnexpectedError", ex.Message, StatusCodes.Status500InternalServerError));
        }
    }

    private async Task SendEmailImmediatelyAsync(string toEmail, string subject, string body)
    {
        using var smtpClient = new System.Net.Mail.SmtpClient(_emailOptions.Host, _emailOptions.Port)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_emailOptions.Email, _emailOptions.Password),
        };

        using var message = new MailMessage(_emailOptions.Email, toEmail, subject, body);
        message.IsBodyHtml = true;
        await smtpClient.SendMailAsync(message);
    }

    [AutomaticRetry(Attempts = 3)]
    public async Task SendEmailInBackground(string toEmail, string subject, string body)
    {
        await SendEmailImmediatelyAsync(toEmail, subject, body);
    }
}


