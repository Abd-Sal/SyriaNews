namespace SyriaNews.Errors;

public class CommentErrors
{
    public static readonly Error NullComment =
    new("Comment.Null",
        "the comment request is null!",
        StatusCodes.Status400BadRequest);

    public static readonly Error NotFoundComment =
        new("Comment.NotFound",
            "the comment is not found!",
            StatusCodes.Status404NotFound);

}
