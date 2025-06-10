namespace SyriaNews.DTOs.Save;

public record FullSaveResponse(
    string Id,
    string MemberID,
    DateTime Date,
    ArticleBreifResponse ArticleBreif
);