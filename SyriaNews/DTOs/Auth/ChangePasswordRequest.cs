namespace SyriaNews.DTOs.Auth;

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword
);
