namespace SyriaNews.HelperTools;

public abstract class ImagesProperties
{
    [Required]
    public string Path { get; set; } = string.Empty;
    [Required]
    public string AllowedExtensions { get; set; } = string.Empty;
    [Required]
    public double MaxSizeInByte { get; set; }
    [Required]
    public double MinSizeInByte { get; set; }
    
}
