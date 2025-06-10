namespace SyriaNews.DTOs.NewsPaper;

public record NewspaperUserResponse(
    string Id,
    string Email,
    string Name,
    string Type,
    DateTime JoinAt,
    int Followers,
    bool IsActive,
    ProfileImageResponse? ProfileImageResponse
) : UserInfo(Id, Email, Type, JoinAt);
