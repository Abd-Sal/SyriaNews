namespace SyriaNews.Errors;

public class AuthErrors
{
    public static readonly Error WrongEmailOrPassword =
        new("Wrong Login",
            "Email/Password is wrong",
            StatusCodes.Status401Unauthorized);
}
