namespace SyriaNews.DTOs.Category;

public record CategoryWithCountOfUseResponse(
    string CategoryName,
    int CountOfUse
);