namespace SyriaNews.Controllers;

[Route("auth")]
[ApiController]
[EnableRateLimiting("Concurrency")]
public class AuthController(IUnitOfWork _unitOfWork, ILogger<AuthController> logger) : ControllerBase
{
    private readonly IUnitOfWork unitOfWork = _unitOfWork;
    private readonly ILogger<AuthController> logger = logger;

    [HttpPost]
    public async Task<IActionResult> LoginRequest
        ([FromBody] LoginRequest loginRequest, CancellationToken cancellationToken = default)
    {
        var check = await unitOfWork.AuthService.GetTokenAsync(loginRequest, cancellationToken);
        if (check.IsSuccess){
            logger.LogInformation("user ({Id}) logged in", User.GetUserId()!);
            return Ok(check.Value);
        }
        return check.ToProblem();
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAsync
        ([FromBody] RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default)
    {
        var check = await unitOfWork.AuthService.GetRefreshTokenAsync(refreshTokenRequest.Token, refreshTokenRequest.RefreshToken, cancellationToken);
        if (check.IsSuccess)
            return Ok(check.Value);
        return check.ToProblem();
    } 
    
    [HttpPost("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefresh
        ([FromBody]RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default)
    {
        var check = await unitOfWork.AuthService.RevokeRefreshTokenAsync(refreshTokenRequest.Token, refreshTokenRequest.RefreshToken, cancellationToken);
        if (check.IsSuccess)
            return Ok(check.Value);
        return check.ToProblem();
    }

    [HttpPost("register-member")]
    public async Task<IActionResult> RegisterMember(
        [FromBody] MemberAddRequest memberAddRequest,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.VisitorService.AddMember(memberAddRequest, cancellationToken);
        if (temp.IsSuccess)
        {
            logger.LogInformation("new member({email}) has registered", memberAddRequest.Email);
            return NoContent();
        }
        return temp.ToProblem();
    }

    [HttpPost("register-newspaper")]
    public async Task<IActionResult> Add(
        [FromBody] NewspaperAddRequest newspaperAddRequest,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.VisitorService.AddNewspaper(newspaperAddRequest, cancellationToken);
        if (temp.IsSuccess)
        {
            logger.LogInformation("new newspaper({email}) has registered", newspaperAddRequest.Email);
            return NoContent();
        }
        return temp.ToProblem();
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ResendConfirmEmailV2(
        [FromQuery] ConfirmationRequest confirmationRequest,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AuthService.ConfirmEmailAsync(confirmationRequest);
        if (temp.IsSuccess)
        {
            logger.LogInformation("user({id}) confirm his account", confirmationRequest.UserID);
            return NoContent();
        }
        return temp.ToProblem();
    }
 
    [HttpPost("resend-confirm-email")]
    public async Task<IActionResult> ResendConfirmEmail(
        [FromBody] ResendEmailConfirmationRequest resendEmailConfirmationRequest,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AuthService.ResendConfirmationEmailAsync(resendEmailConfirmationRequest);
        if (temp.IsSuccess)
        {
            logger.LogInformation("user({Email}) request to resend confirmation email", resendEmailConfirmationRequest.Email);
            return NoContent();
        }
        return temp.ToProblem();
    }
    
    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword(
        [FromBody] ForgetPasswordRequest forgetPasswordRequest,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AuthService.SendResetPasswordCodeAsync(forgetPasswordRequest.Email);
        if (temp.IsSuccess)
        {
            logger.LogInformation("user({id}) get code to reset password", forgetPasswordRequest.Email);
            return NoContent();
        }
        return temp.ToProblem();
    }
    
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest resetPasswordRequest,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AuthService.ResetPasswordAsync(resetPasswordRequest);
        if (temp.IsSuccess)
        {
            logger.LogInformation("user({id}) reset his password", resetPasswordRequest.Email);
            return NoContent();
        }
        return temp.ToProblem();
    }
}
