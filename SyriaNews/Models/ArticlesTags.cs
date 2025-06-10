namespace SyriaNews.Models;

public class ArticlesTags
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string ArticleID { get; set; } = string.Empty;
    public string TagID { get; set; } = string.Empty;

    public Article Article { get; set; } = default!;
    public Tag Tag{ get; set; } = default!;
}
