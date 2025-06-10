namespace SyriaNews.DTOs.Article;

public record ArticleUpdateRequest(
    string Title,
    string Description,
    string Content,
    string CategoryID
);
