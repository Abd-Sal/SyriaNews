namespace SyriaNews.DTOs.Article;

public record ArticleRequest(
    string Title,
    string Description,
    string Content,
    string CategoryID
);
