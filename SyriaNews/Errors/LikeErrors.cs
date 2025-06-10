namespace SyriaNews.Errors;

public class LikeErrors
{
    public static readonly Error NullLike =
        new("Like.Null",
            "the like is null!",
            StatusCodes.Status400BadRequest);

    public static readonly Error NotFoundLike =
        new("Like.NotFound",
            "the like is not found!",
            StatusCodes.Status404NotFound);

    public static readonly Error DuplicatedLike =
        new("Like.Duplicated",
            "the like is duplicated!",
            StatusCodes.Status409Conflict);
}
