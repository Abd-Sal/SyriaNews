namespace SyriaNews.Models;

public class ApplicationUser : IdentityUser
{
    public string TypeUser { get; set; } = UserTypes.Member.ToString();
    public DateTime JoinAt { get; set; } = DateTime.UtcNow;

    public virtual ProfileImage? ProfileImage { get; set; }
    public virtual NewsPaper? NewsPaper { get; set; }
    public virtual Admin? Admin { get; set; }
    public virtual Member? Member { get; set; }

    public List<RefreshToken> refreshTokens { get; set; } = [];

}
