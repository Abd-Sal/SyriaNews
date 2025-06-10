namespace SyriaNews.Repository.Implementations;

public class JwtProvider(IOptions<JwtOption> _option) : IJwtProvider
{
    private readonly JwtOption option = _option.Value;
    public (string token, int expiresIn) GenerateToke(ApplicationUser user, string role)
    {
        Claim[] claims =
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Typ, user.TypeUser),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString()),
            new(nameof(role), role)
        };

        var symmetriceSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(option.Key));

        var signingCredentials = new SigningCredentials(symmetriceSecurityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: option.Issuer,
            audience: option.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(option.ExpiryMinute),
            signingCredentials: signingCredentials
            );

        return (token: new JwtSecurityTokenHandler().WriteToken(token), option.ExpiryMinute * 60);
    }

    public string? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var symmetriceSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(option.Key));

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = symmetriceSecurityKey,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;
            return jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
        }
        catch
        {
            return null;
        }

        throw new NotImplementedException();
    }
}
