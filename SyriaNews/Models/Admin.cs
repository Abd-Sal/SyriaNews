namespace SyriaNews.Models;

public class Admin
{
    public string UserID { get; set; } = string.Empty;

    public ApplicationUser User { get; set; } = default!;
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

}
