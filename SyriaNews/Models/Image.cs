namespace SyriaNews.Models;

public class Image
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string Name { get; set; } = string.Empty;
    public int Placement { get; set; }
    public string ArticleID { get; set; } = string.Empty;

    public Article Article { get; set; } = default!;
}
