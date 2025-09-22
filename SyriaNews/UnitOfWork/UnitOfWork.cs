namespace SyriaNews.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    public UnitOfWork(
        AppDbContext appDbContext,
        IMapper mapper,
        UserManager<ApplicationUser> applicationUser,
        IJwtProvider jwtProvider,
        RoleManager<ApplicationRole> roleManager,
        HybridCache hybridCache,
        SignInManager<ApplicationUser> signInManager,
        IOptions<ArticleImages> articleImagesOptions,
        IOptions<ProfileImages> profileImagesOption,
        IWebHostEnvironment webHostEnvironment,
        ILogger<VisitorService> visitorLogger,
        INotificationSender notificationSender
    )
    {
        UserService = new UserService(applicationUser, appDbContext, mapper);
        AuthService = new AuthService
            (applicationUser, jwtProvider, signInManager, appDbContext, notificationSender);
        VisitorService = new VisitorService
            (appDbContext, mapper, applicationUser, hybridCache,
            UserService, profileImagesOption, articleImagesOptions, webHostEnvironment, visitorLogger,
            notificationSender);
        NewsPaperService = new NewsPaperService(appDbContext, mapper, hybridCache, VisitorService,
             articleImagesOptions, profileImagesOption);
        MemberService = new MemberService(appDbContext, VisitorService, profileImagesOption, mapper, hybridCache);
        AdminService = new AdminService(applicationUser, appDbContext, NewsPaperService, MemberService, VisitorService, mapper, roleManager, hybridCache);
    }
    public IAuthService AuthService { get; } 
    public INewsPaperService NewsPaperService { get; }
    public IMemberService MemberService { get; }
    public IAdminService AdminService { get; }
    public IUserService UserService { get; }
    public IVisitorService VisitorService { get; }
}