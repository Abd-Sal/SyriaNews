namespace SyriaNews.DTOs.Admin;

public record AdminUserResponse(
    string Id,
    string Email,
    string Type,
    DateTime JoinAt
) : UserInfo(Id, Email, Type, JoinAt);

