namespace SyriaNews.Models;

public class Tag
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string TagName { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.UtcNow;

    public ICollection<ArticlesTags> ArticlesTags { get; set; } = new List<ArticlesTags>();
}
