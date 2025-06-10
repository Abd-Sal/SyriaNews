namespace SyriaNews.Errors;

public class RoleErrors
{
    public static readonly Error DuplicatedRole =
        new("Role.Duplicated",
            "role is duplicated!",
            StatusCodes.Status409Conflict);

    public static readonly Error NotFoundRole =
        new("Role.NotFound",
            "role is not found!",
            StatusCodes.Status404NotFound);
}
