namespace SyriaNews.Health;

//public class MailProviderHealthCheck(IOptions<MailSettings> mailOptions) : IHealthCheck
//{
//    private readonly MailSettings mailOptions = mailOptions.Value;

//    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
//    {
//		try
//		{
//			using var smtp = new System.Net.Mail.SmtpClient();
//			smtp.Connect(mailOptions.Host, mailOptions.Port, SecureSocketOptions.StartTls);
//			smtp.Authenticate(mailOptions.Mail, mailOptions.Password, cancellationToken);

//			return await Task.FromResult(HealthCheckResult.Healthy());
//		}
//		catch (Exception ex)
//		{
//            return await Task.FromResult(HealthCheckResult.Unhealthy(exception: ex));
//        }
//    }
//}
