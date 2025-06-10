namespace SyriaNews.DTOs.Save;

public record SaveResponse(
    string Id,
    string MemberID,
    string ArticleID,
    DateTime Date
);
