namespace SyriaNews.DTOs.Auth;

public record AuthResponse(
    string Id,
    string Email,
    string Token,
    string UserType,
    int ExpireIn,
    string RefreshToken,
    DateTime RefreshTokenExpiration
);
