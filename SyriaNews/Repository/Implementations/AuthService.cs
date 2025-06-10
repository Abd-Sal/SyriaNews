namespace SyriaNews.Repository.Implementations;

public class AuthService(
    UserManager<ApplicationUser> _userManager,
    IJwtProvider _jwtProvider,
    SignInManager<ApplicationUser> signInManager,
    IOptions<MailSettings> mailerSettingOptions,
    AppDbContext appDbContext
    ) : IAuthService
{
    private readonly UserManager<ApplicationUser> userManager = _userManager;
    private readonly IJwtProvider jwtProvider = _jwtProvider;
    private readonly SignInManager<ApplicationUser> signInManager = signInManager;
    private readonly AppDbContext appDbContext = appDbContext;
    private readonly MailSettings mailerSettingOptions = mailerSettingOptions.Value;
    private readonly int _refreshTokenExpiryDays = 30;
    public async Task<Result<AuthResponse>> GetTokenAsync
        (LoginRequest loginRequest, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(loginRequest.email);
        if (user is null)
            return Result.Failure<AuthResponse>(AuthErrors.WrongEmailOrPassword);

        var result = await signInManager.PasswordSignInAsync(user, loginRequest.password, false, false);
        if(!result.Succeeded)
        {
            return Result.Failure<AuthResponse>(result.IsNotAllowed ? UserErrors.EmailNotConfirmedUser : UserErrors.InvalidcredentialsUser);
        }
        var role = await userManager.GetRolesAsync(user);
        var token = jwtProvider.GenerateToke(user, role.First());

        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

        user.refreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiration,
        });
        await userManager.UpdateAsync(user);

        return Result.Success(new AuthResponse
            (user.Id, user.Email!, token.token, user.TypeUser, token.expiresIn, refreshToken, refreshTokenExpiration));
    }

    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = jwtProvider.ValidateToken(token);
        if (userId is null)
            return Result.Failure<AuthResponse>(new Error("NotAuthorized", "your are not authorized", StatusCodes.Status401Unauthorized));

        var user = await userManager.FindByIdAsync(userId);
        if(user is null)
            return Result.Failure<AuthResponse>(new Error("NotAuthorized", "your are not authorized", StatusCodes.Status401Unauthorized));

        var userRefreshToken = user.refreshTokens
            .SingleOrDefault(x => x.Token == refreshToken &&
            x.IsActive);
        if(userRefreshToken is null)
            return Result.Failure<AuthResponse>(new Error("NotAuthorized", "your are not authorized", StatusCodes.Status401Unauthorized));

        userRefreshToken.RevokedOn = DateTime.UtcNow;
        var role = await userManager.GetRolesAsync(user);
        var newToken = jwtProvider.GenerateToke(user, role.First());

        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

        user.refreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = refreshTokenExpiration,
        });
        await userManager.UpdateAsync(user);

        return Result.Success(new AuthResponse
            (user.Id, user.Email!, newToken.token, user.TypeUser, newToken.expiresIn, newRefreshToken, refreshTokenExpiration));

    }

    public async Task<Result<bool>> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = jwtProvider.ValidateToken(token);
        if (userId is null)
            return Result.Failure<bool>(new Error("NotAuthorized", "your are not authorized", StatusCodes.Status401Unauthorized));

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure<bool>(new Error("NotAuthorized", "your are not authorized", StatusCodes.Status401Unauthorized));

        var userRefreshToken = user.refreshTokens
            .SingleOrDefault(x => x.Token == refreshToken &&
            x.IsActive);
        if (userRefreshToken is null)
            return Result.Failure<bool>(new Error("NotAuthorized", "your are not authorized", StatusCodes.Status401Unauthorized));

        userRefreshToken.RevokedOn = DateTime.UtcNow;
        await userManager.UpdateAsync(user);
        return Result.Success(true);
    }

    public async Task<Result> ConfirmEmailAsync(ConfirmationRequest confirmationRequest)
    {
        if (await userManager.FindByIdAsync(confirmationRequest.UserID) is not { } user)
            return Result.Failure(UserErrors.InvalidCodeUser);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmationUser);

        var code = confirmationRequest.Code;
        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Result.Failure(UserErrors.InvalidCodeUser);
        }
        var result = await userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
        {
            if (user.TypeUser == UserTypes.Member.ToString())
                await appDbContext.Members.Where(x => x.UserID == user.Id)
                    .ExecuteUpdateAsync(setters =>
                        setters
                            .SetProperty(x => x.IsActive, true)
                    );
            else if(user.TypeUser == UserTypes.NewsPaper.ToString())
                await appDbContext.NewsPapers.Where(x => x.UserID == user.Id)
                    .ExecuteUpdateAsync(setters =>
                        setters
                            .SetProperty(x => x.IsActive, true)
                    );
            return Result.Success();
        }

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ResendConfirmationEmailAsync(ResendEmailConfirmationRequest resendEmailConfirmationRequest)
    {
        if (await userManager.FindByEmailAsync(resendEmailConfirmationRequest.Email) is not { } user)
            return Result.Success();

        if(user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmationUser);

        string name = await getName(user);

        var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var origin = ConstantStrings.Origin;
        BackgroundJob.Enqueue(() =>
            EmailSendingHelp.SendEmailAsync($"{name}", user, code, origin!, mailerSettingOptions)
        );
        await Task.CompletedTask;

        return Result.Success();
    }

    private static string GenerateRefreshToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    public async Task<Result> SendResetPasswordCodeAsync(string email)
    {
        if (await userManager.FindByEmailAsync(email) is not { } user)
            return Result.Success();

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmedUser);

        string name = await getName(user);

        var code = await userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var origin = ConstantStrings.Origin;

        await EmailSendingHelp.SendResetPasswordEmailAsync($"{name}", user, code, origin!, mailerSettingOptions);

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest)
    {
        var user = await userManager.FindByEmailAsync(resetPasswordRequest.Email);
        if (user is null || !user.EmailConfirmed)
            return Result.Failure(UserErrors.InvalidCodeUser);

        IdentityResult result;
        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetPasswordRequest.Code));
            result = await userManager.ResetPasswordAsync(user, code, resetPasswordRequest.NewPassword);
        }
        catch(FormatException)
        {
            result = IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken());
        }
        if (result.Succeeded)
            return Result.Success();
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status401Unauthorized));
    }

    private async Task<string> getName(ApplicationUser user)
    {
        if(user.TypeUser == UserTypes.Member.ToString())
        {
            var member = (await appDbContext.Members.FindAsync(user.Id));
            return $"{member!.FirstName} {member.LastName}";
        }else if(user.TypeUser == UserTypes.NewsPaper.ToString()) {
            var newspaper = (await appDbContext.NewsPapers.FindAsync(user.Id));
            return $"{newspaper!.Name}";
        }
        return ":)";
    }

}
