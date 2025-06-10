namespace SyriaNews.DTOs.Article;

public record AddFullArticleRequest(
    ArticleRequest ArticleRequest,
    List<TagRequest> Tags,
    List<FullImageRequest> Images
);
