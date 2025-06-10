namespace SyriaNews.DTOs.NewsPaper;

public record NewspaperAddRequest(
    string Email,
    string Password,
    string Name
);
