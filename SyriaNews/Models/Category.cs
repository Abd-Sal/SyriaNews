namespace SyriaNews.Models;

public class Category
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string CategoryName { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.UtcNow;

    public ICollection<Article> Articles { get; set; } = new List<Article>();
}

