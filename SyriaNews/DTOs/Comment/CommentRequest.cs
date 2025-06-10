namespace SyriaNews.DTOs.Comment;

public record CommentRequest(
    string Content,
    string ArticleID
);
