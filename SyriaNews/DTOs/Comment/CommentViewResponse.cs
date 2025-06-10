namespace SyriaNews.DTOs.Comment;

public record CommentViewResponse(
    string Id,
    string Content,
    string ArticleID,
    MemberBreifResponseWithProfileImage Member,
    DateTime Date
);

