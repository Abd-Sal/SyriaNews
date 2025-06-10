namespace SyriaNews.Errors;

public class ImageErrors
{
    public static readonly Error NullImage =
        new("Image.Null",
            "the image is null!",
            StatusCodes.Status400BadRequest);

    public static readonly Error NotFoundImage =
        new("Image.NotFound",
            "the image is not found!",
            StatusCodes.Status404NotFound);

    public static readonly Error DuplicatedImage =
        new("Image.Duplicated",
            "the image is already exist!",
            StatusCodes.Status409Conflict);

    public static readonly Error DuplicatedPlacement =
        new("Image.DuplicatedPlacement",
            "the image place you entered is already token!",
            StatusCodes.Status409Conflict);
    
    public static Error TooManyImages(int imageCount) =>
        new("Image.TooManyImages",
            $"the maximum images count (${imageCount})!",
            StatusCodes.Status409Conflict);

    public static Error SizeImageError(double minSize, double maxSize) =>
        new("Image.SizeError",
            $"the image size have to be between (${minSize/1024d/1024d} & ${maxSize/1024d/1024d})MB",
            StatusCodes.Status400BadRequest);

    public static Error ImageExtensionError(string allowedExtensions) =>
        new("Image.ExtensionError",
            $"the image extension have to be (${allowedExtensions})MB",
            StatusCodes.Status400BadRequest);

    public static readonly Error ImagePathError =
        new("Image.PathError",
            "the image path is wrong!",
            StatusCodes.Status400BadRequest);
}
