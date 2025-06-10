namespace SyriaNews.DTOs.Common;

public record ResultFilter
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
