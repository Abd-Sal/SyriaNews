namespace SyriaNews.Models;

public class Save
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string ArticleID { get; set; } = string.Empty;
    public string MemberID { get; set; } = string.Empty;
    public DateTime Date { get; set; }

    public Article Article { get; set; } = default!;
    public Member Member { get; set; } = default!;

}

