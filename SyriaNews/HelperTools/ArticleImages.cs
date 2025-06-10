namespace SyriaNews.HelperTools;

public class ArticleImages : ImagesProperties
{
    public static string sectionName = "Images:ArticleImages";

    [Required]
    public int MaximumImageCount { get; set; }
}
