namespace SyriaNews.DTOs.Auth;

public record LoginRequest(
    string email,
    string password
);
