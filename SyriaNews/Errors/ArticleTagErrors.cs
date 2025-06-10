namespace SyriaNews.Errors;

public class ArticleTagErrors
{
    public static readonly Error  NullArticleTag =
        new("ArticleTag.Null",
            "the article tag is null!",
            StatusCodes.Status400BadRequest);

    public static readonly Error NotFoundArticleTag =
        new("ArticleTag.NotFound",
            "the article tag is not found!",
            StatusCodes.Status404NotFound);

    public static readonly Error DuplicatedArticleTag =
        new("ArticleTag.Duplicated",
            "the article tag is duplicated!",
            StatusCodes.Status409Conflict);

}
