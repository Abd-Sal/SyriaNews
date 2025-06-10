namespace SyriaNews.Errors;

public class NewspaperErrors
{
    public static readonly Error NullNewspaper =
        new("Newspaper.Null",
            "the newspaper is null!",
            StatusCodes.Status400BadRequest);

    public static readonly Error NotFoundNewspaper =
        new("Newspaper.NotFound",
            "the newspaper is Not Found",
            StatusCodes.Status404NotFound);

    public static readonly Error NotActicatedNewspaper =
        new("Newspaper.NotActicated",
            "the newspaper is not activated",
            StatusCodes.Status401Unauthorized);

    public static readonly Error CannotAddingNewspaper =
        new("Newspaper.CouldNotAdding",
            "the newspaper could not added for unexpected error",
            StatusCodes.Status400BadRequest);

    public static readonly Error NewspaperNameDuplicated =
        new("Newspaper.DuplicatedName",
            "the newspaper name is duplicated",
            StatusCodes.Status409Conflict);
}
