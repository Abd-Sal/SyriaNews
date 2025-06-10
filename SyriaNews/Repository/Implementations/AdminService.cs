namespace SyriaNews.Repository.Implementations;

public class AdminService(
    UserManager<ApplicationUser> userManager,
    AppDbContext appDbContext,
    INewsPaperService newsPaperService,
    IMemberService memberService,
    IVisitorService visitorService,
    IMapper mapper,
    RoleManager<ApplicationRole> roleManager,
    HybridCache hybridCache
    ) : IAdminService
{
    private readonly UserManager<ApplicationUser> userManager = userManager;
    private readonly AppDbContext appDbContext = appDbContext;
    private readonly INewsPaperService newsPaperService = newsPaperService;
    private readonly IMemberService memberService = memberService;
    private readonly IVisitorService visitorService = visitorService;
    private readonly IMapper mapper = mapper;
    private readonly RoleManager<ApplicationRole> roleManager = roleManager;
    private readonly HybridCache hybridCache = hybridCache;


    //A1
    public async Task<Result<bool>> ToggleNewspaperStatus
        (string newpaperID, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.NewsPapers.FindAsync(newpaperID, cancellationToken) is not { } newspaper)
            return Result.Failure<bool>(NewspaperErrors.NotFoundNewspaper);

        var simpleNotificationMessage = new SimpleNotificationRequest("",
            ConstantStrings.BlockUser.Title, ConstantStrings.BlockUser.Message);

        var notificationNewspapre = NotificationBlockedUser(newpaperID, simpleNotificationMessage, cancellationToken);

        var result = await appDbContext.NewsPapers
            .Where(x => x.UserID == newpaperID)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.IsActive, !newspaper.IsActive)
            );

        var doneNotification = await notificationNewspapre;

        return Result.Success(!newspaper.IsActive);
    }

    //A2
    public async Task<Result<bool>> ToggleMemberStatus
        (string memberID, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.Members.FindAsync(memberID, cancellationToken) is not { } member)
            return Result.Failure<bool>(MemberErrors.NotFoundMember);

        var simpleNotificationMessage = new SimpleNotificationRequest("",
            ConstantStrings.BlockUser.Title, ConstantStrings.BlockUser.Message);

        var notificationMember = NotificationBlockedUser(memberID, simpleNotificationMessage, cancellationToken);

        var result = await appDbContext.Members
            .Where(x => x.UserID == memberID)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.IsActive, !member.IsActive)
            );

        var doneNotification = await notificationMember;

        return Result.Success(!member.IsActive);
    }

    //A3
    public async Task<Result<bool>> TogglePostStatus
        (string articleID, CancellationToken cancellationToken = default)
        => await newsPaperService.TogglePostStatus(null, articleID, cancellationToken);

    //A4
    public async Task<Result> ConfirmUserAccount
        (string userID, UserTypes userTypes, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userID);
        if (user is null || user.TypeUser != userTypes.ToString())
            return Result.Failure(UserErrors.NotFoundUser);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmationUser);

        var result = userManager.Users.Where(x => x.Id == userID)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.EmailConfirmed, true)
            );

        return Result.Success();
    }

    //A5
    public async Task<Result<PaginatedList<ArticleBreifResponse>>> SeeNewArticles
        (bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await visitorService.ShowNewArticlesByPostDate(false, resultFilter, cancellationToken);

    //A6
    public async Task<Result<PaginatedList<ArticleBreifResponse>>> SeeNewArticlesByCategory
        (string categoryID, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await visitorService.ShowNewArticlesByCategory(categoryID, isPosted, resultFilter, cancellationToken);

    //A7
    public async Task<Result<PaginatedList<ArticleBreifResponse>>> SeeNewArticlesByTag
        (string tagName, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await visitorService.ShowNewArticlesByTag(tagName, isPosted, resultFilter, cancellationToken);

    //A8
    public async Task<Result<PaginatedList<ArticleBreifResponse>>> SeeNewArticlesTitle
        (string title, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await visitorService.ShowNewArticlesByTitle(title, isPosted, resultFilter, cancellationToken);

    //A9
    public async Task<Result<PaginatedList<ArticleBreifResponse>>> SeeNewArticlesByViews
        (bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await visitorService.ShowNewArticlesByMostViewed(isPosted, resultFilter, cancellationToken);

    //A10
    public async Task<Result<PaginatedList<NewspaperUserResponse>>> SearchForNewspaperByName
        (string newspaperName, bool isActive, ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await visitorService.SearchForNewspaper(newspaperName, isActive, resultFilter, cancellationToken);

    //A11
    public async Task<Result<NewspaperUserResponse>> SeeNewspaperProfile
        (string newspaperID, bool isActive, CancellationToken cancellationToken = default)
        => await visitorService.NewspaperProfile(newspaperID, isActive, cancellationToken);

    //A12
    public async Task<Result<PaginatedList<ArticleBreifResponse>>> SeeArticlesForNewspaper
        (string newspaperID, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await visitorService.ArticleByNewspaper(newspaperID, isPosted, resultFilter, cancellationToken);

    //A13
    public async Task<Result<PaginatedList<CommentViewResponse>>> CommentsForArticle
        (string articleID, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await visitorService.CommentsForArticle(articleID, isPosted, resultFilter, cancellationToken);

    //A14
    public async Task<Result<PaginatedList<MemberUserResponse>>> ShowArticlesLikes
        (string articleID, ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => (await appDbContext.Articles.FindAsync(articleID, cancellationToken) is not { } article)
        ? Result.Failure<PaginatedList<MemberUserResponse>>(ArticleErrors.NotFoundArticle)
        : await visitorService.ShowMembersLikes(article.NewsPaperID, articleID, false, resultFilter, cancellationToken);

    //A15
    public async Task<Result<PaginatedList<FullCommentResponse>>> MemberComments
        (string memberID, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await memberService.MemberComments(memberID, isPosted, resultFilter, cancellationToken);

    //A16
    public async Task<Result<PaginatedList<NewspaperUserResponse>>> MemeberFollows
        (string memberID, ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await memberService.FollowedNewspapers(memberID, resultFilter, cancellationToken);

    //A17
    public async Task<Result<PaginatedList<FullSaveResponse>>> MemeberSaves
        (string memberID, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await memberService.FullSavedArtciels(memberID, isPosted, resultFilter, cancellationToken);

    //A18
    public async Task<Result<PaginatedList<ArticleBreifResponse>>> MemeberLikes
        (string memberID, bool isActive, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await memberService.LikedArticle(memberID, isPosted, resultFilter, cancellationToken);

    //A19
    public async Task<Result<MemberUserResponse>> SeeMemberProfile
        (string memberID, bool isActive, CancellationToken cancellationToken = default)
        => await visitorService.MemberProfile(memberID, isActive, cancellationToken);

    //A20
    public async Task<Result> RemoveComment
        (string commentID, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.Comments.FindAsync(commentID, cancellationToken) is not { } comment)
            return Result.Failure(CommentErrors.NotFoundComment);

        var article = await appDbContext.Articles.Where(x => x.Id == comment.ArticleID)
            .SingleAsync(cancellationToken);

        var update = await appDbContext.Articles.Where(x => x.Id == article.Id)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.AllComments, (article.AllComments - 1))
            );

        var result = await appDbContext.Comments.Where(x => x.Id == commentID)
            .ExecuteDeleteAsync(cancellationToken);
        return Result.Success();
    }

    //A21
    public async Task<Result<FullArticleFullResponse>> ReadArticle
        (string userAdminID, string articleID, bool isPosted, CancellationToken cancellationToken = default)
        => await visitorService.ReadArticle(userAdminID, articleID, isPosted, cancellationToken);   //Edit

    //A22
    public async Task<Result<CategoryResponse>> AddCategory
        (CategoryRequest categoryRequest, CancellationToken cancellationToken = default)
    {

        if (await appDbContext.Categories.AnyAsync(x => x.CategoryName == categoryRequest.CategoryName.Trim(), cancellationToken))
            return Result.Failure<CategoryResponse>(CategoryErrors.DuplicatedCategory);
        var category = categoryRequest.ToCategory(mapper);
        await appDbContext.Categories.AddAsync(category, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);
        await hybridCache.RemoveByTagAsync(nameof(GetAllCategories));
        return Result.Success(category.ToCategoryResponse(mapper));
    }

    //A23
    public async Task<Result> UnpublishArticlesByCategory
        (string categoryID, CancellationToken cancellationToken = default)
    {
        if (!(await appDbContext.Categories.AnyAsync(x => x.Id == categoryID, cancellationToken)))
            return Result.Failure(CategoryErrors.NotFoundCategory);

        var result = await appDbContext.Articles.Where(x => x.CategoryID == categoryID)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.IsPosted, false)
            );
        return Result.Success();
    }

    //A24
    public async Task<Result<TagResponse>> AddTag
        (TagRequest tagRequest, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.Tags.AnyAsync(x => x.TagName == tagRequest.TagName.Trim(), cancellationToken))
            return Result.Failure<TagResponse>(TagErrors.DuplicatedTag);

        var tag = tagRequest.ToTag(mapper);
        await appDbContext.Tags.AddAsync(tag, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(tag.ToTagResponse(mapper));
    }

    //A25
    public async Task<Result<PaginatedList<CategoryResponse>>> GetAllCategories
        (ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await visitorService.GetAllCategories(resultFilter, cancellationToken);

    //A26
    public async Task<Result<(string imagePath, string contentType)>> GetProfileImage
        (string userID, UserTypes userTypes, bool isActive, CancellationToken cancellationToken = default)
        => await visitorService.GetProfileImage(userID, userTypes, isActive, cancellationToken);

    //A27
    public async Task<Result<(string imagePath, string contentType)>> GetArticleImage
        (string imageName, bool isPosted, CancellationToken cancellationToken)
        => await visitorService.GetArticleImage(imageName, isPosted, cancellationToken);

    //A28
    public async Task<Result<RoleResponse>> AddRole
        (RoleRequest roleRequest, CancellationToken cancellationToken = default)
    {
        var temp = await appDbContext.Roles.Where(x => x.Name == roleRequest.name && x.IsDeleted)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.IsDeleted, false)
            );
        if (temp > 0)
            return Result.Success((await appDbContext.Roles.SingleAsync(x => x.Name == roleRequest.name, cancellationToken)).ToRoleResponse(mapper));

        if (await appDbContext.Roles.AnyAsync(x => x.Name == roleRequest.name.Trim() && !x.IsDeleted, cancellationToken))
            return Result.Failure<RoleResponse>(RoleErrors.DuplicatedRole);

        var role = new ApplicationRole
        {
            Name = roleRequest.name,
            NormalizedName = roleRequest.name.ToUpper(),
            ConcurrencyStamp = Guid.CreateVersion7().ToString()
        };

        await roleManager.CreateAsync(role);
        return Result.Success(role.ToRoleResponse(mapper));
    }
    
    //A29
    public async Task<Result<PaginatedList<RoleResponse>>> GetAllRoles
        (bool isDeleted, ResultFilter resultFilter, CancellationToken cancellationToken = default)
    {
        var data = appDbContext.Roles.AsNoTracking()
            .Where(x => isDeleted ? x.IsDeleted : true)
            .OrderBy(x => x.Name)
            .Select(x => x.ToRoleResponse(mapper));
        var result = await PaginatedList<RoleResponse>.CreateAsync(data, resultFilter.PageNumber, resultFilter.PageSize, cancellationToken);
        return Result.Success(result);
    }
    
    //A30
    public async Task<Result<RoleResponse>> GetRoleByID
        (string id, bool isDeleted = true, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.Roles.FindAsync(id, cancellationToken) is not { } role)
            return Result.Failure<RoleResponse>(RoleErrors.NotFoundRole);
        if (isDeleted && role.IsDeleted)
            return Result.Failure<RoleResponse>(RoleErrors.NotFoundRole);
        return Result.Success(role.ToRoleResponse(mapper));
    }

    //A31
    public async Task<Result> DeleteRole
        (string id, CancellationToken cancellationToken = default)
    {
        if(!(await appDbContext.Roles.AnyAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)))
            return Result.Failure(RoleErrors.NotFoundRole);
        await appDbContext.Roles.Where(x => x.Id == id)
            .ExecuteUpdateAsync(setters =>
                setters
                .SetProperty(x => x.IsDeleted, true)
            );
        return Result.Success();
    }

    //A32
    public async Task<Result<PaginatedList<NotificationResponse>>> GetAllNotifications
        (string userID, bool isReaded, ResultFilter resultFilter, CancellationToken cancellationToken = default)
    {
        var data = appDbContext.Notifications.AsNoTracking()
            .Where(x => x.ReceiveUserID == userID)
            .OrderByDescending(x => x.CreatedAt).ThenBy(x => x.IsReaded)
            .Select(x => new NotificationResponse(x.Id, x.Title, x.Message, x.NotificationTypes, x.EntityID, x.EntityType, x.CreatedAt, x.IsReaded));
        var result = await PaginatedList<NotificationResponse>.CreateAsync(data, resultFilter.PageNumber, resultFilter.PageSize, cancellationToken);
        return  Result.Success(result);
    }

    //A33
    public async Task<Result> ReadNotification
        (string userID, string notificationID, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.Notifications.FindAsync(notificationID, cancellationToken) is not { } notification)
            return Result.Failure(NotificationErrors.NotificationNotFound);

        if (notification.ReceiveUserID != userID || await appDbContext.Users.AnyAsync(x => x.Id == userID && x.TypeUser != UserTypes.Admin.ToString(), cancellationToken))
            return Result.Failure(NotificationErrors.NotAllowedNotification);

        if (notification.IsReaded)
            return Result.Failure(NotificationErrors.NotificationAlreadyReaded);

        await appDbContext.Notifications.Where(x => x.Id == notificationID)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.IsReaded, true)
            );

        return Result.Success();
    }

    //A34
    public async Task<Result> ReadAllNotifications
        (string userID, CancellationToken cancellationToken = default)
    {
        await appDbContext.Notifications
            .Where(x => x.ReceiveUserID == userID && !x.IsReaded)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.IsReaded, true)
            );
        return Result.Success();
    }

    //A35
    public async Task<Result<int>> CountOfNotifications
        (string userID, bool isReaded = true, CancellationToken cancellationToken = default)
        => Result.Success((await appDbContext.Notifications.AsNoTracking()
                .CountAsync(x => x.ReceiveUserID == userID && isReaded ? !x.IsReaded : true)));

    private async Task<Result> NotificationBlockedUser
        (string userID, SimpleNotificationRequest simpleNotificationRequest, CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            NotificationTypes = NotificationTypes.BlockNotification.ToString(),
            Title = simpleNotificationRequest.Title,
            Message = simpleNotificationRequest.Message,
            EntityID = simpleNotificationRequest.EntityID,
            EntityType = EntitiesTypes.User.ToString(),
            SenderUserID = "",
            ReceiveUserID = userID,
        };

        await appDbContext.Notifications.AddAsync(notification, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
