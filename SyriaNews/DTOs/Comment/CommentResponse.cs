namespace SyriaNews.DTOs.Comment;

public record CommentResponse(
    string Id,
    string Content,
    string ArticleID,
    string MemberID,
    DateTime Date
);

