namespace SyriaNews.DTOs.Auth;

public record ResetPasswordRequest(
    string Email,
    string Code,
    string NewPassword
);