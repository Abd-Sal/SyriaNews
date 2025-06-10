namespace SyriaNews.Models;

public class Notification
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string NotificationTypes { get; set; } = string.Empty;
    public string ReceiveUserID { get; set; } = string.Empty;
    public string SenderUserID { get; set; } = string.Empty;
    public string? EntityID { get; set; }
    public string? EntityType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsReaded { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public NewsPaper? NewsPaper { get; set; }
    public Member? Member { get; set; }
    public Admin? Admin { get; set; }

}


