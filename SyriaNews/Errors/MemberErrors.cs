namespace SyriaNews.Errors;

public class MemberErrors
{
    public static readonly Error NullMember =
        new("Member.Null",
            "the member is null!",
            StatusCodes.Status400BadRequest);

    public static readonly Error NotFoundMember =
        new("Member.NotFound",
            "the member is not found!",
            StatusCodes.Status404NotFound);

    public static readonly Error NotActivatedMember =
        new("Member.NotActivated",
            "the member is not active!",
            StatusCodes.Status401Unauthorized);
}
