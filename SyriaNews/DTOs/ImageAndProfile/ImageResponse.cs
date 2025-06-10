namespace SyriaNews.DTOs.ImageAndProfile;

public record ImageResponse(
  string Id,
  string Name,
  int Placement,
  string ArticleID
);
