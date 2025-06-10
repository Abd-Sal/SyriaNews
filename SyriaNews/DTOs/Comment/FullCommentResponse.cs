namespace SyriaNews.DTOs.Comment;

public record FullCommentResponse(
    string Id,
    string Content,
    ArticleBreifResponse Article,
    MemberBreifResponse Member,
    DateTime Date
);

