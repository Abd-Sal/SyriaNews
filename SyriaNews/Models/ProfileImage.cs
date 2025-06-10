namespace SyriaNews.Models;

public class ProfileImage
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string Name { get; set; } = string.Empty;
    public string userID { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.UtcNow;

    public ApplicationUser User { get; set; } = default!;
}
