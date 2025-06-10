namespace SyriaNews.DTOs.Like;

public record LikeResponse(
    string Id,
    string MemberID,
    string ArticleID,
    DateTime Date
);
