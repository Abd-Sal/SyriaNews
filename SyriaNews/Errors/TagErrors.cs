namespace SyriaNews.Errors;

public class TagErrors
{
    public static readonly Error NullTag =
        new("Tag.Null",
            "the tag is null!",
            StatusCodes.Status400BadRequest);
    
    public static readonly Error NotFoundTag =
        new("Tag.NotFound",
            "the tag is not found!",
            StatusCodes.Status404NotFound);

    public static readonly Error DuplicatedTag =
        new("Tag.Duplicated",
            "the tag is duplicated!",
            StatusCodes.Status409Conflict);

    public static readonly Error TooManyTag =
        new("Tag.TooManyTag ",
            "the tags count should be less than 50!",
            StatusCodes.Status409Conflict);

}
