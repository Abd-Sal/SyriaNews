namespace SyriaNews.Errors;

public class UserErrors
{
    public static readonly Error NotFoundUser =
        new("User.NotFound",
            "the user is not found!",
            StatusCodes.Status404NotFound);

    public static readonly Error UnauthorizedUser =
        new("User.Unauthorized",
            "the user is unauthorized!",
            StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidcredentialsUser =
        new("User.Invalidcredentials",
            "invalid user credentials!",
            StatusCodes.Status404NotFound);

    public static readonly Error DuplicatedEmailUser =
        new("User.DuplicatedEmail",
            "the user email is duplicated!",
            StatusCodes.Status409Conflict);

    public static readonly Error InvalidCodeUser =
        new("User.InvalidCode",
            "the code is invalid!",
            StatusCodes.Status400BadRequest);

    public static readonly Error DuplicatedConfirmationUser =
        new("User.DuplicatedConfirmation",
            "the email is already confirmed!",
            StatusCodes.Status409Conflict);

    public static readonly Error EmailNotConfirmedUser =
        new("User.EmailNotConfirmed",
            "email is not confirmed!",
            StatusCodes.Status409Conflict);
}
