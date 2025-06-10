namespace SyriaNews.DTOs.Article;

public record FullArticleFullResponse(
    string Id,
    string Title,
    string Description,
    string Content,
    NewspaperBreifResponse Newspaper,
    CategoryResponse Category,
    bool IsPosted,
    DateTime PostDate,
    int Likes,
    int Views,
    int Comments,
    List<TagResponse> Tags,
    List<ImageResponse> Images
);
