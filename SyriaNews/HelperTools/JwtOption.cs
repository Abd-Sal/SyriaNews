using System.ComponentModel.DataAnnotations;

namespace SyriaNews.HelperTools;

public class JwtOption
{
    public static string SectionName = "Jwt";

    [Required]
    public string Key { get; init; } = string.Empty;

    [Required]
    public string Issuer { get; init; } = string.Empty;

    [Required]
    public string Audience { get; init; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int ExpiryMinute { get; init; }
}
