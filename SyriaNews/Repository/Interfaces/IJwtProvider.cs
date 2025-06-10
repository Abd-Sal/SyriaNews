namespace SyriaNews.Repository.Interfaces;

public interface IJwtProvider
{
    (string token, int expiresIn) GenerateToke(ApplicationUser user, string Role);
    string? ValidateToken(string token);
}
