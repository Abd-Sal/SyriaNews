namespace SyriaNews.Repository.Implementations;

public class UserService(
    UserManager<ApplicationUser> userManager,
    AppDbContext appDbContext,
    IMapper mapper) : IUserService
{
    private readonly UserManager<ApplicationUser> userManager = userManager;
    private readonly AppDbContext appDbContext = appDbContext;
    private readonly IMapper mapper = mapper;

    public async Task<Result<(object result, UserTypes userType)>> Me
        (string Id, CancellationToken cancellationToken = default)
    {
        if (!await appDbContext.Users.AnyAsync(x => x.Id == Id, cancellationToken))
            return Result.Failure<(object, UserTypes)>(UserErrors.NotFoundUser);
        var checkType = await getType(Id);
        if (checkType.userType == UserTypes.Member.ToString())
        {
            var result = await MemInfo(Id, cancellationToken);
            return result.IsSuccess
                ? Result.Success<(object, UserTypes)>((result.Value, UserTypes.Member))
                : Result.Failure<(object, UserTypes)>(result.Error);
        }
        else if (checkType.userType == UserTypes.NewsPaper.ToString())
        {
            var result = await NewsInfo(Id, cancellationToken);
            return result.IsSuccess
                ? Result.Success<(object, UserTypes)>((result.Value, UserTypes.NewsPaper))
                : Result.Failure<(object, UserTypes)>(result.Error);
        }
        else
        {
            var result = await AdminInfo(Id);
            return result.IsSuccess
                ? Result.Success<(object, UserTypes)>((result.Value, UserTypes.Admin))
                : Result.Failure<(object, UserTypes)>(result.Error);
        }
    }

    private async Task<Result<MemberUserResponse>> MemInfo
        (string Id, CancellationToken cancellationToken = default)
    {
        var result = await (from usr in userManager.Users.AsNoTracking().Select(x => new { x.Id, x.Email, x.TypeUser, x.JoinAt })
                            join mem in appDbContext.Members.AsNoTracking()
                            on usr.Id equals mem.UserID
                            where mem.UserID == Id
                            select new MemberUserResponse(
                             usr.Id, usr.Email, mem.FirstName, mem.LastName,
                             (mem.Gender ? "Female" : "Male"), usr.TypeUser, usr.JoinAt, mem.IsActive,
                             (from profileImage in appDbContext.ProfileImages.AsNoTracking()
                              where profileImage.userID == usr.Id
                              select profileImage).SingleOrDefault().ToProfileImageResponse(mapper)
                           ))
                      .FirstOrDefaultAsync(cancellationToken);
        return result is not null
            ? Result.Success(result)
            : Result.Failure<MemberUserResponse>(MemberErrors.NotFoundMember);
    }
    private async Task<Result<NewspaperUserResponse>> NewsInfo
        (string Id, CancellationToken cancellationToken = default)
    {
        var result = await (from usr in userManager.Users.AsNoTracking().Select(x => new { x.Id, x.Email, x.TypeUser, x.JoinAt })
                            join news in appDbContext.NewsPapers.AsNoTracking()
                            on usr.Id equals news.UserID
                            where news.UserID == Id
                            select new NewspaperUserResponse(
                             usr.Id, usr.Email, news.Name, usr.TypeUser, usr.JoinAt, news.followers, news.IsActive,
                                 (from profileImage in appDbContext.ProfileImages.AsNoTracking()
                                  where profileImage.userID == usr.Id
                                  select profileImage).SingleOrDefault().ToProfileImageResponse(mapper)
                           ))
                      .FirstOrDefaultAsync(cancellationToken);
        return result is not null
            ? Result.Success(result)
            : Result.Failure<NewspaperUserResponse>(MemberErrors.NotFoundMember);
    }
    private async Task<Result<AdminUserResponse>> AdminInfo
        (string Id)
    {
        var results = (from usr in userManager.Users.AsNoTracking().Select(x => new { x.Id, x.Email, x.TypeUser, x.JoinAt })
                       join adm in appDbContext.Admins.AsNoTracking()
                       on usr.Id equals adm.UserID
                       where adm.UserID == Id
                       select new AdminUserResponse(
                        usr.Id, usr.Email, usr.TypeUser, usr.JoinAt
                      ));
        return Result.Success(await results.FirstAsync());
    }

    private async Task<(string Id, string userType)> getType(string Id)
    {
        var temp = await userManager.Users
            .Select(x => new { Id = x.Id, userType = x.TypeUser }).SingleAsync(x => x.Id == Id);
        return (temp.Id, temp.userType);
    }

    public async Task<Result> ChangePassword
        (string Id, ChangePasswordRequest changePasswordRequest)
    {
        var user = await userManager.FindByIdAsync(Id);
        var result = await userManager.ChangePasswordAsync(user!, changePasswordRequest.CurrentPassword, changePasswordRequest.NewPassword);
        if (result.Succeeded)
        {
            await userManager.RemoveAuthenticationTokenAsync(user!, "SyriaNews", "BearerToken");
            var refreshTokenList = user!.refreshTokens;
            await appDbContext.Database.ExecuteSqlRawAsync(
                "DELETE FROM [RefreshTokens] WHERE [UserID] = {0}",
                Id);
            return Result.Success();
        }
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
}
