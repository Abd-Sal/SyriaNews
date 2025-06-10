namespace SyriaNews.DTOs;

public abstract record UserInfo(
    string Id,
    string Email,
    string Type,
    DateTime JoinAt
);