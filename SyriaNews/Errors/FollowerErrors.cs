namespace SyriaNews.Errors;

public class FollowerErrors
{
    public static readonly Error NullFollower =
        new("Follower.Null",
            "the follower request is null!",
            StatusCodes.Status400BadRequest);

    public static readonly Error NotFoundFollower =
        new("Follower.NotFound",
            "the follower is not found!",
            StatusCodes.Status404NotFound);

    public static readonly Error DuplicatedFollower =
        new("Follower.Duplicated",
            "the follower is duplicated!",
            StatusCodes.Status404NotFound);
}
