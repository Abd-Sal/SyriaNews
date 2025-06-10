namespace SyriaNews.DTOs.Article;

public record ArticleBreifResponse(
    string Id,
    string Title,
    string Description,
    NewspaperBreifResponse Newspaper,
    CategoryResponse Category,
    bool IsPosted,
    DateTime PostDate,
    int Likes,
    int Views,
    int Comments,
    List<TagResponse> Tags,
    ImageResponse? Image
);
