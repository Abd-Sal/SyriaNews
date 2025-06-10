namespace SyriaNews.DTOs.Auth;

public record RefreshTokenRequest(
    string  Token,
    string RefreshToken
);