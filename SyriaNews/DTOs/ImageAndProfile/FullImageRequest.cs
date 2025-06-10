namespace SyriaNews.DTOs.ImageAndProfile;

public record FullImageRequest(
    int Placement,
    IFormFile File
);