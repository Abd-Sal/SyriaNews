namespace SyriaNews.Models;

public class Article
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string Title { get; set; } = string.Empty;
    public string Descrpition { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string NewsPaperID { get; set; } = string.Empty;
    public string CategoryID { get; set; } = string.Empty;
    public bool IsPosted { get; set; } = true;
    public DateTime PostDate { get; set; } = DateTime.UtcNow;
    public int AllLikes { get; set; } = 0;
    public int Views { get; set; } = 0;
    public int AllComments { get; set; } = 0;
    public DateTime? LastUpdate { get; set; }

    public NewsPaper NewsPaper { get; set; } = default!;
    public Category Category { get; set; } = default!;
    public ICollection<Save> Saves { get; set; } = new List<Save>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<Image> Images { get; set; } = new List<Image>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<ArticlesTags> ArticlesTags { get; set; } = new List<ArticlesTags>();

}

