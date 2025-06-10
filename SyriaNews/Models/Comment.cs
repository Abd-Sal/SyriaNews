namespace SyriaNews.Models;

public class Comment
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string Content { get; set; } = string.Empty;
    public string ArticleID { get; set; } = string.Empty;
    public string MemberID { get; set; } = string.Empty;
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastUpdate { get; set; }

    public Article Article { get; set; } = default!;
    public Member Member { get; set; } = default!;
}

