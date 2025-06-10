namespace SyriaNews.HelperTools;

public class ImageHelper
{
    public static async Task<Result<(bool statusProcess, string imageName)>> Save
        (IFormFile image, ImagesProperties imagesProperties)
    {

        if (!Directory.Exists(imagesProperties.Path))
            Directory.CreateDirectory(imagesProperties.Path);

        var fileName = $"{Guid.CreateVersion7().ToString()}-{image.FileName}";
        var filePath = $"{imagesProperties.Path}\\{fileName}";
        if (File.Exists(filePath))
            return Result.Failure<(bool, string)>(ImageErrors.DuplicatedImage);

        if(image.Length < imagesProperties.MinSizeInByte || image.Length > imagesProperties.MaxSizeInByte)
            return Result.Failure<(bool, string)>(ImageErrors.SizeImageError(imagesProperties.MinSizeInByte, imagesProperties.MaxSizeInByte));

        var mem = new MemoryStream();
        await image.CopyToAsync(mem);
        await File.Create(filePath).DisposeAsync();
        await File.WriteAllBytesAsync(filePath, mem.ToArray());

        var directoryInfo = new DirectoryInfo(filePath);
        if (!imagesProperties.AllowedExtensions.Contains(directoryInfo.Extension.ToLower()))
        {
            var delete = Delete(filePath);
            if (delete.IsSuccess)
                return Result.Failure<(bool,string)>(ImageErrors.ImageExtensionError(imagesProperties.AllowedExtensions));
            return Result.Failure<(bool,string)>(delete.Error);
        }
        mem.Close();
        return Result.Success((true, fileName));
    }
    public static Result<bool> Delete(string path)
    {
        if (!File.Exists(path))
            return Result.Failure<bool>(ImageErrors.ImagePathError);
        File.Delete(path);
        return Result.Success(true);
    }

    public static string GetContentType(string path)
    {
        var extension = Path.GetExtension(path).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };
    }
}
