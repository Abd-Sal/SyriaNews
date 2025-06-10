namespace SyriaNews.DTOs.Member;

public record MemberUserResponse(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    string Gender,
    string Type,
    DateTime JoinAt,
    bool IsActive,
    ProfileImageResponse? ProfileImageResponse
) : UserInfo(Id, Email, Type, JoinAt);
