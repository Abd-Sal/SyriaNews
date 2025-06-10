namespace SyriaNews.Models;

public class Member
{
    public string UserID { get; set; }  = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool Gender { get; set; }
    public bool IsActive { get; set; } = false;

    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Save> Saves { get; set; } = new List<Save>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<Follower> Followers { get; set; } = new List<Follower>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ApplicationUser User { get; set; } = default!;
}
