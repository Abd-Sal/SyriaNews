namespace SyriaNews.Models;

public class Follower
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string NewsPaperID { get; set; } = string.Empty;
    public string MemberID { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.UtcNow;

    public NewsPaper NewsPaper { get; set; } = default!;
    public Member Member { get; set; } = default!;
}

