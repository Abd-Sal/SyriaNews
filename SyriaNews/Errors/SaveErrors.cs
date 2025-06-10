namespace SyriaNews.Errors;

public class SaveErrors
{
    public static readonly Error NullSave =
        new("Save.Null",
            "the save request is null!",
            StatusCodes.Status400BadRequest);

    public static readonly Error NotFoundSave =
        new("Save.NotFound",
            "the save is not found!",
            StatusCodes.Status404NotFound);

    public static readonly Error DuplicatedSave =
        new("Save.Duplicated",
            "the save is duplicated!",
            StatusCodes.Status404NotFound);
}
