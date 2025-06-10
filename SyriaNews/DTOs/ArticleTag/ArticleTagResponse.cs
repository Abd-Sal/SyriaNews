namespace SyriaNews.DTOs.ArticleTag;

public record ArticleTagResponse(
    string Id,
    string ArticleID,
    string TagID
);