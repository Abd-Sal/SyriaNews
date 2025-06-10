namespace SyriaNews.DTOs.Auth;

public record ConfirmationRequest(
    string UserID,
    string Code
);