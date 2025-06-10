namespace SyriaNews.DTOs.NewsPaper;

public record NewspaperBreifResponse(
    string Id,
    string Name,
    int Followers,
    bool IsActive
);
