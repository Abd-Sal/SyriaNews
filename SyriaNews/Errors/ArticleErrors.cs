namespace SyriaNews.Errors;

public class ArticleErrors
{
    public static readonly Error NullArticle =
        new("Article.Null",
            "the article is null!",
            StatusCodes.Status400BadRequest);

    public static readonly Error NotFoundArticle =
        new("Article.NotFound",
            "the article is not found!",
            StatusCodes.Status404NotFound);
    
    public static readonly Error DuplicatedTagInArticle =
        new("Article.DuplicatedTagInArticle ",
            "the tag already exist in article!",
            StatusCodes.Status409Conflict);
}
