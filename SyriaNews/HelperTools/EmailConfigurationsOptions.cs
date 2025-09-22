namespace SyriaNews.HelperTools;

public class EmailConfigurationsOptions
{
    public static readonly string SectionName = "Email_Configurations";

    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
    [Required]
    public string Host { get; set; } = string.Empty;
    [Range(1,10000)]
    public int Port { get; set; }
}
