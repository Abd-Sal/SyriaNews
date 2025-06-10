namespace SyriaNews.DTOs.Like;

public record FullLikeResponse(
    string Id,
    MemberBreifResponse Member,
    ArticleBreifResponse Article,
    DateTime Date
);