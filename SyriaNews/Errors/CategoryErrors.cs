namespace SyriaNews.Errors;

public class CategoryErrors
{
    public static readonly Error NullCategory =
        new("Category.Null",
            "the category is null!",
            StatusCodes.Status400BadRequest);

    public static readonly Error DuplicatedCategory =
        new("Category.Duplicated",
            "the category is duplicated!",
            StatusCodes.Status409Conflict);

    public static readonly Error NotFoundCategory =
        new("Category.NotFound",
            "the category is not found!",
            StatusCodes.Status404NotFound);
}
