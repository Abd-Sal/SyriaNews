namespace SyriaNews.Repository.Interfaces;

public interface IAuthService
{
    public Task<Result<AuthResponse>> GetTokenAsync
        (LoginRequest loginRequest, CancellationToken cancellationToken = default);
    public Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    public Task<Result<bool>> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    public Task<Result> ConfirmEmailAsync(ConfirmationRequest confirmationRequest);
    public Task<Result> ResendConfirmationEmailAsync(ResendEmailConfirmationRequest resendEmailConfirmationRequest);
    public Task<Result> SendResetPasswordCodeAsync(string email);
    public Task<Result> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest);
}
