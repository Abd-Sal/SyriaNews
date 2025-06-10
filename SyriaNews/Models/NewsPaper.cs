namespace SyriaNews.Models;

public class NewsPaper
{
    public string UserID { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int followers { get; set; }
    public bool IsActive { get; set; } = false;

    public ApplicationUser User { get; set; } = default!;
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Follower> Followers { get; set; } = new List<Follower>();
    public ICollection<Article> Articles { get; set; } = new List<Article>();

}
