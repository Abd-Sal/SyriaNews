namespace SyriaNews.Models;

public class Like
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string ArticleID { get; set; } = string.Empty;
    public string MemberID { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.UtcNow;

    public Article Article { get; set; } = default!;
    public Member Member{ get; set; } = default!;
}

