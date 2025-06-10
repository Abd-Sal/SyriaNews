using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SyriaNews.Repository.Implementations;

public class VisitorService(
    AppDbContext appDbContext,
    IMapper mapper,
    UserManager<ApplicationUser> userManager,
    HybridCache hybridCache,
    IUserService userService,
    IOptions<ProfileImages> profileImages,
    IOptions<ArticleImages> articleImageOptions,
    IWebHostEnvironment webHostEnvironment,
    IOptions<MailSettings> mailerSettingOptions,
    ILogger<VisitorService> logger
    ) : IVisitorService
{
    private readonly AppDbContext appDbContext = appDbContext;
    private readonly IMapper mapper = mapper;
    private readonly UserManager<ApplicationUser> userManager = userManager;
    private readonly HybridCache hybridCache = hybridCache;
    private readonly IUserService userService = userService;
    private readonly IWebHostEnvironment webHostEnvironment = webHostEnvironment;
    private readonly ILogger<VisitorService> logger = logger;
    private readonly MailSettings mailerSettingOptions = mailerSettingOptions.Value;
    private readonly ArticleImages articleImageOptions = articleImageOptions.Value;
    private readonly ProfileImages profileImagesOptions = profileImages.Value;
    private readonly string prefixCacheVisitor = nameof(prefixCacheVisitor);

    //V1
    public async Task<Result> AddNewspaper
        (NewspaperAddRequest newspaperAddRequest, CancellationToken cancellationToken = default)
    {
        if ((await appDbContext.NewsPapers.AnyAsync(x => x.Name == newspaperAddRequest.Name)))
            return Result.Failure(NewspaperErrors.NewspaperNameDuplicated);

        if ((await userManager.Users.AnyAsync(x => x.Email == newspaperAddRequest.Email, cancellationToken)))
            return Result.Failure(UserErrors.DuplicatedEmailUser);

        logger.LogInformation("Start register newspaper");
        var appUser = new ApplicationUser
        {
            Email = newspaperAddRequest.Email,
            UserName = newspaperAddRequest.Email,
            TypeUser = UserTypes.NewsPaper.ToString(),
        };
        var addedUser = await userManager.CreateAsync(appUser, newspaperAddRequest.Password);

        if (!addedUser.Succeeded)
        {
            var error = addedUser.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        var code = await userManager.GenerateEmailConfirmationTokenAsync(appUser);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var origin = ConstantStrings.Origin;

        logger.LogInformation("Send confirmation code to newspaper({id})", appUser.Id);
        BackgroundJob.Enqueue(() => 
            EmailSendingHelp.SendEmailAsync($"{newspaperAddRequest.Name}", appUser, code, origin!, mailerSettingOptions)
        );
        await Task.CompletedTask;

        var addedNewspaper = await appDbContext.NewsPapers.AddAsync(new NewsPaper
        {
            Name = newspaperAddRequest.Name,
            UserID = appUser.Id
        }, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);

        await userManager.AddToRoleAsync(appUser, DefaultRoles.Newspaper);

        logger.LogInformation("Newspaper({id}) registeration is done", appUser.Id);

        await hybridCache.RemoveByTagAsync("search_newspaper");

        return Result.Success();
    }

    //V2
    public async Task<Result> AddMember
        (MemberAddRequest memberAddRequest, CancellationToken cancellationToken = default)
    {
        if (await userManager.Users.AnyAsync(x => x.Email == memberAddRequest.Email, cancellationToken))
            return Result.Failure(UserErrors.DuplicatedEmailUser);

        logger.LogInformation("Start register member");
        var appUser = new ApplicationUser
        {
            Email = memberAddRequest.Email,
            UserName = memberAddRequest.Email,
            TypeUser = UserTypes.Member.ToString(),
        };
        var userAdded = await userManager.CreateAsync(appUser, memberAddRequest.Password);
        if (!userAdded.Succeeded)
        {
            var error = userAdded.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        var code = await userManager.GenerateEmailConfirmationTokenAsync(appUser);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var origin = ConstantStrings.Origin;

        logger.LogInformation("Send confirmation code to member({id})", appUser.Id);
        BackgroundJob.Enqueue(() =>
            EmailSendingHelp.SendEmailAsync($"{memberAddRequest.FirstName} {memberAddRequest.LastName}", appUser, code, origin!, mailerSettingOptions)
        );
        await Task.CompletedTask;

        var addedMember = await appDbContext.Members.AddAsync(new Member
        {
            UserID = appUser.Id,
            FirstName = memberAddRequest.FirstName,
            LastName = memberAddRequest.LastName,
            Gender = memberAddRequest.Gender,
        }, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);

        await userManager.AddToRoleAsync(appUser, DefaultRoles.Member);

        logger.LogInformation("Member({id}) registeration is done", appUser.Id);
        return Result.Success();
    }

    //V3
    public async Task<Result<PaginatedList<ArticleBreifResponse>>> ShowNewArticlesByPostDate
        (bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default)
    {
        var articlesKey = $"{prefixCacheVisitor}/{nameof(ShowNewArticlesByPostDate)}/{isPosted}";
        var cache = await hybridCache.GetOrCreateAsync(
            articlesKey,
            async entry =>
                await GetArticles(isPosted, "PostDate", cancellationToken),
            cancellationToken: cancellationToken,
            tags: ["show_articles"]
        );

        var result = PaginatedList<ArticleBreifResponse>.Create(cache, resultFilter.PageNumber, resultFilter.PageSize);
        return Result.Success(result);
    }

    //V4
    public async Task<Result<PaginatedList<ArticleBreifResponse>>> ShowNewArticlesByMostViewed
        (bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default)
    {
        var articlesKey = $"{prefixCacheVisitor}/{nameof(ShowNewArticlesByMostViewed)}/{isPosted}";
        var cache = await hybridCache.GetOrCreateAsync(
            articlesKey,
            async entry =>
                await GetArticles(isPosted, "Views", cancellationToken),
            cancellationToken: cancellationToken,
            tags: ["show_articles"]
        );
        var result =  PaginatedList<ArticleBreifResponse>.Create(cache, resultFilter.PageNumber, resultFilter.PageSize);
        return Result.Success(result);
    }

    //V5
    public async Task<Result<PaginatedList<ArticleBreifResponse>>> ShowNewArticlesByTag
        (string tagName, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.Tags.SingleOrDefaultAsync(x => x.TagName.Contains(tagName.Trim()), cancellationToken) is not { } tag)
            return Result.Failure<PaginatedList<ArticleBreifResponse>>(TagErrors.NotFoundTag);
        
        var articlesKey = $"{prefixCacheVisitor}/{nameof(ShowNewArticlesByTag)}/{tagName}/{isPosted}";
        var cache = await hybridCache.GetOrCreateAsync(
            articlesKey,
            async entry =>
            {
                var articles = (await appDbContext.ArticlesTags.AsNoTracking().Where(x => x.TagID == tag.Id)
                    .Join(appDbContext.Articles.AsNoTracking(),
                        at => at.ArticleID,
                        a => a.Id,
                        (at, a) => a
                    )
                    .Where(x => isPosted ? x.IsPosted : true)
                    .OrderByDescending(x => x.PostDate)
                    .ToListAsync(cancellationToken));
                var result = new List<ArticleBreifResponse>();
                foreach (var article in articles)
                    result.Add(await article.ToArticleBreifResponse(appDbContext, mapper, cancellationToken));
                return result;
            },
            cancellationToken: cancellationToken,
            tags: ["show_articles"]
        );
        var result = PaginatedList<ArticleBreifResponse>.Create(cache, resultFilter.PageNumber, resultFilter.PageSize);
        return Result.Success(result);
    }

    //V6
    public async Task<Result<PaginatedList<ArticleBreifResponse>>> ShowNewArticlesByCategory
        (string categoryID, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default)
    {
        if (!(await appDbContext.Categories.AnyAsync(x => x.Id == categoryID, cancellationToken)))
            return Result.Failure<PaginatedList<ArticleBreifResponse>>(CategoryErrors.NotFoundCategory);

        var articlesKey = $"{prefixCacheVisitor}/{nameof(ShowNewArticlesByCategory)}/{categoryID}/{isPosted}";
        var cache = await hybridCache.GetOrCreateAsync(
            articlesKey,
            async entry =>
            {
                var articles = (await appDbContext.Articles.AsNoTracking()
                    .Where(x => x.CategoryID == categoryID && (isPosted ? x.IsPosted : true))
                    .ToListAsync(cancellationToken))
                    .OrderByDescending(x => x.PostDate);
                var result = new List<ArticleBreifResponse>();
                foreach (var article in articles)
                    result.Add(await article.ToArticleBreifResponse(appDbContext, mapper, cancellationToken));
                return result;
            },
            cancellationToken: cancellationToken,
            tags: ["show_articles"]
        );
        var result = PaginatedList<ArticleBreifResponse>.Create(cache, resultFilter.PageNumber, resultFilter.PageSize);
        return Result.Success(result);
    }

    //V7
    public async Task<Result<PaginatedList<ArticleBreifResponse>>> ShowNewArticlesByTitle
        (string title, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default)
    {
        var articlesKey = $"{prefixCacheVisitor}/{nameof(ShowNewArticlesByTitle)}/{title}/{isPosted}";
        var cache = await hybridCache.GetOrCreateAsync(
            articlesKey,
            async entry =>
            {
                var articles = (await appDbContext.Articles.AsNoTracking()
                    .Where(x => x.IsPosted && x.Title.Contains(title.Trim()) && (isPosted ? x.IsPosted : true))
                    .OrderByDescending(x => x.PostDate).ThenBy(x => x.Title)
                    .ToListAsync(cancellationToken));
                var result = new List<ArticleBreifResponse>();
                foreach (var article in articles)
                    result.Add(await article.ToArticleBreifResponse(appDbContext, mapper, cancellationToken));
                return result;
            },
            cancellationToken: cancellationToken,
            tags: ["show_articles_by_title", "show_articles"]
        );

        var result = PaginatedList<ArticleBreifResponse>.Create(cache, resultFilter.PageNumber, resultFilter.PageSize);
        return Result.Success(result);
    }

    //V8
    public async Task<Result<PaginatedList<NewspaperUserResponse>>> SearchForNewspaper
        (string newspaperName, bool isActive, ResultFilter resultFilter, CancellationToken cancellationToken = default)
    {
        var searchNewspaperKey = $"{prefixCacheVisitor}/{nameof(SearchForNewspaper)}/{newspaperName}/{isActive}";
        var cache = await hybridCache.GetOrCreateAsync(
            searchNewspaperKey,
            async entry => {
                var temp = await appDbContext.NewsPapers.AsNoTracking()
                    .Where(x => x.Name.Contains(newspaperName.Trim()) && (isActive ? x.IsActive : true))
                    .OrderByDescending(x => x.followers).ThenBy(x => x.Name)
                    .Join(appDbContext.Users.AsNoTracking().Select(x => new { x.Id, x.Email, x.TypeUser, x.JoinAt }),
                        newspaper => newspaper.UserID,
                        user => user.Id,
                        (newspaper, user) => new { newspaper, user }
                    )
                    .Select(x => new NewspaperUserResponse(x.user.Id, x.user.Email!, x.newspaper.Name, x.user.TypeUser, x.user.JoinAt,
                        x.newspaper.followers, x.newspaper.IsActive,
                        (from profileImage in appDbContext.ProfileImages.AsNoTracking()
                         where profileImage.userID == x.user.Id
                         select profileImage).SingleOrDefault().ToProfileImageResponse(mapper)))
                    .ToListAsync(cancellationToken);
                return temp;
            },
            cancellationToken: cancellationToken,
            tags: ["search_newspaper"]
        );
        var result = PaginatedList<NewspaperUserResponse>.Create(cache, resultFilter.PageNumber, resultFilter.PageSize);
        return Result.Success(result);
    }

    //V9
    public async Task<Result<FullArticleFullResponse>> ReadArticle
        (string? userID, string articleID, bool isPosted = true, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.Articles.FindAsync(articleID, cancellationToken) is not { } article)
            return Result.Failure<FullArticleFullResponse>(ArticleErrors.NotFoundArticle);
        
        var articleKey = $"{prefixCacheVisitor}/{nameof(ReadArticle)}/{userID??"NoID"}/{articleID}/{isPosted}";
        if (userID is not null)
        {
            if (await appDbContext.Users.FindAsync(userID, cancellationToken) is not { } user)
                return Result.Failure<FullArticleFullResponse>(ArticleErrors.NotFoundArticle);

            if (user.TypeUser == UserTypes.Member.ToString() && !article.IsPosted)
                return Result.Failure<FullArticleFullResponse>(ArticleErrors.NotFoundArticle);

            if (user.TypeUser == UserTypes.Member.ToString())
            {
                await appDbContext.Articles.Where(x => x.Id == articleID)
                    .ExecuteUpdateAsync(setters =>
                        setters.SetProperty(x => x.Views, (article.Views) + 1)
                    );
                await hybridCache.RemoveByTagAsync($"count_views_by_categories_{article.NewsPaperID}");
                await hybridCache.RemoveByTagAsync($"count_of_views_{article.NewsPaperID}");
                await hybridCache.RemoveByTagAsync($"read_{article.Id}");
            }
        }
        else
        {
            if (article.IsPosted)
            {
                var newViews = ++article.Views;
                await appDbContext.Articles.Where(x => x.Id == articleID)
                    .ExecuteUpdateAsync(setters =>
                        setters.SetProperty(x => x.Views, newViews)
                    );
                await hybridCache.RemoveByTagAsync($"count_views_by_categories_{article.NewsPaperID}");
                await hybridCache.RemoveByTagAsync($"count_of_views_{article.NewsPaperID}");
                await hybridCache.RemoveByTagAsync($"read_{article.Id}");
            }
        }

        var cache = await hybridCache.GetOrCreateAsync(
            articleKey,
            async entry =>{
                var fullArticle = new FullArticleFullResponse(
                        article.Id, article.Title, article.Descrpition, article.Content,
                        (await appDbContext.NewsPapers.SingleAsync(x => x.UserID == article.NewsPaperID, cancellationToken)).ToNewspaperBreifResponse(mapper),
                        (await appDbContext.Categories.SingleAsync(x => x.Id == article.CategoryID, cancellationToken)).ToCategoryResponse(mapper),
                        article.IsPosted, article.PostDate, article.AllLikes, article.Views, article.AllComments,
                        (await appDbContext.ArticlesTags.AsNoTracking().Where(x => x.ArticleID == articleID)
                            .Join(appDbContext.Tags.AsNoTracking(),
                                at => at.TagID,
                                t => t.Id,
                                (at, t) => t.ToTagResponse(mapper)
                            ).ToListAsync(cancellationToken)),
                        (await appDbContext.Images.AsNoTracking().Where(x => x.ArticleID == article.Id).ToListAsync(cancellationToken))
                        .ToImagesResponse(mapper).ToList()
                    );
                return fullArticle;
            },
            cancellationToken: cancellationToken,
            tags: ["read_article", $"read_{articleID}"]
        );

        return Result.Success(cache);
    }

    //V10
    public async Task<Result<PaginatedList<CommentViewResponse>>> CommentsForArticle
        (string articleID, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default)
    {
        if (!(await appDbContext.Articles.AnyAsync(x => x.Id == articleID && isPosted ? x.IsPosted : true, cancellationToken)))
            return Result.Failure<PaginatedList<CommentViewResponse>>(ArticleErrors.NotFoundArticle);

        var commentsKey = $"{prefixCacheVisitor}/{nameof(CommentsForArticle)}/{articleID}/{isPosted}";
        var cache = await hybridCache.GetOrCreateAsync(
            commentsKey,
            async entry =>
            {

                var temp = await (
                    from comment in appDbContext.Comments.AsNoTracking()
                    join article in appDbContext.Articles.AsNoTracking()
                        on comment.ArticleID equals article.Id
                    where article.Id == articleID
                    join member in appDbContext.Members.AsNoTracking()
                        on comment.MemberID equals member.UserID
                    join profileImage in appDbContext.ProfileImages.AsNoTracking()
                        on member.UserID equals profileImage.userID into profileImages
                    from profileImage in profileImages.DefaultIfEmpty() // Left join
                    select new
                    {
                        Comment = comment,
                        Member = member,
                        ProfileImage = profileImage
                    }
                ).ToListAsync(cancellationToken); // Fetch data first

                // Now map to DTOs in memory
                var result = temp.Select(x => new CommentViewResponse(
                    x.Comment.Id,
                    x.Comment.Content,
                    x.Comment.ArticleID,
                    new MemberBreifResponseWithProfileImage(
                        mapper.Map<MemberBreifResponse>(x.Member),
                        x.ProfileImage != null ? mapper.Map<ProfileImageResponse>(x.ProfileImage) : null
                    ),
                    x.Comment.CreateDate
                )).ToList();

                return result;
            },
            cancellationToken: cancellationToken,
            tags: ["comments_for_article", $"comments_for_article_{articleID}"]
        );

        var result = PaginatedList<CommentViewResponse>.Create(cache, resultFilter.PageNumber, resultFilter.PageSize);
        return Result.Success(result);
    }

    //V11
    public async Task<Result<NewspaperUserResponse>> NewspaperProfile
        (string newspaperID, bool isActive = true, CancellationToken cancellationToken = default)
    {
        var temp = await userService.Me(newspaperID, cancellationToken);
        if (temp.IsFailure || temp.Value.userType != UserTypes.NewsPaper)
            return Result.Failure<NewspaperUserResponse>(NewspaperErrors.NotFoundNewspaper);
        var result = temp.Value.result as NewspaperUserResponse;
        if (result is null)
            return Result.Failure<NewspaperUserResponse>(NewspaperErrors.NullNewspaper);
        var IsAvtice = isActive ? result.IsActive : true;
        if (!IsAvtice)
            return Result.Failure<NewspaperUserResponse>(NewspaperErrors.NotActicatedNewspaper);
        return Result.Success(result);
    }
    
    //V12
    public async Task<Result<MemberUserResponse>> MemberProfile
        (string memberID, bool isActive = true, CancellationToken cancellationToken = default)
    {
        var temp = await userService.Me(memberID, cancellationToken);
        if (temp.IsFailure || temp.Value.userType != UserTypes.Member)
            return Result.Failure<MemberUserResponse>(MemberErrors.NotFoundMember);
        var result = temp.Value.result as MemberUserResponse;
        if (result is null)
            return Result.Failure<MemberUserResponse>(MemberErrors.NullMember);
        var IsAvtice = isActive ? result.IsActive : true;
        if (!IsAvtice)
            return Result.Failure<MemberUserResponse>(MemberErrors.NotActivatedMember);
        return Result.Success(result);
    }

    //V13
    public async Task<Result<PaginatedList<MemberUserResponse>>> ShowMembersLikes
        (string? newspaperID, string articleID, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default)
    {
        if (newspaperID is not null)
        {
            if (!(await appDbContext.Articles.AnyAsync(x => x.Id == articleID && x.NewsPaperID == newspaperID, cancellationToken)))
                return Result.Failure<PaginatedList<MemberUserResponse>>(ArticleErrors.NotFoundArticle);
            isPosted = false;
        }

        if (!(await appDbContext.Articles.AnyAsync(x => x.Id == articleID && isPosted ? x.IsPosted : true, cancellationToken)))
            return Result.Failure<PaginatedList<MemberUserResponse>>(ArticleErrors.NotFoundArticle);

        var showMemberLikes = $"{prefixCacheVisitor}/{nameof(ShowMembersLikes)}/{newspaperID ?? "NoID"}/{articleID}/{isPosted}";
        var cache = await hybridCache.GetOrCreateAsync(
            showMemberLikes,
            async entry =>
            {
                var members = await appDbContext.Likes.AsNoTracking()
                    .Include(x => x.Member)
                        .ThenInclude(x => x.User)
                            .ThenInclude(x => x.ProfileImage)
                    .Where(x => x.ArticleID == articleID)
                    .OrderByDescending(x => x.Date).ThenBy(x => x.Member.FirstName).ThenBy(x => x.Member.LastName)
                    .Select(x => new MemberUserResponse(x.Member.UserID, x.Member.User.Email!, x.Member.FirstName, x.Member.LastName,
                                                        x.Member.Gender ? "Female" : "Male", x.Member.User.TypeUser, x.Member.User.JoinAt,
                                                        x.Member.IsActive, x.Member.User.ProfileImage.ToProfileImageResponse(mapper)))
                    .ToListAsync(cancellationToken);
                return members;
            },
            cancellationToken: cancellationToken,
            tags: ["show_member_likes", $"show_member_likes_{articleID}"]
        );

        var result = PaginatedList<MemberUserResponse>.Create(cache, resultFilter.PageNumber, resultFilter.PageSize);
        return Result.Success(result);
    }

    //V14
    public async Task<Result<PaginatedList<MemberBreifResponseWithProfileImage>>> ShowMemberFollowing
        (string newspaperID, bool isActive, ResultFilter resultFilter, CancellationToken cancellationToken = default)
    {
        if (!(await appDbContext.NewsPapers.AnyAsync(x => x.UserID == newspaperID && isActive ? x.IsActive : true, cancellationToken)))
            return Result.Failure<PaginatedList<MemberBreifResponseWithProfileImage>>(NewspaperErrors.NotFoundNewspaper);

        var memberFollowingKey = $"{prefixCacheVisitor}/{nameof(ShowMemberFollowing)}/{newspaperID}/{isActive}";
        var cache = await hybridCache.GetOrCreateAsync(
            memberFollowingKey,
            async entry =>
                await appDbContext.Followers.AsNoTracking()
                    .Include(x => x.Member)
                        .ThenInclude(x => x.User)
                            .ThenInclude(x => x.ProfileImage)
                    .Where(x => x.NewsPaperID == newspaperID)
                    .OrderByDescending(x => x.Date).ThenBy(x => x.Member.FirstName).ThenBy(x => x.Member.FirstName)
                    .Select(x => new MemberBreifResponseWithProfileImage
                        (x.Member.ToMemberBreifResponse(mapper), x.Member.User.ProfileImage.ToProfileImageResponse(mapper))
                    ).ToListAsync(cancellationToken),
            cancellationToken: cancellationToken,
            tags: ["show_member_following"]
        );

        var result = PaginatedList<MemberBreifResponseWithProfileImage>.Create(cache, resultFilter.PageNumber, resultFilter.PageSize);
        return Result.Success(result);
    }

    //V15
    public async Task<Result<PaginatedList<ArticleBreifResponse>>> ArticleByNewspaper
        (string newspaperID, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default)
    {
        if (!(await appDbContext.NewsPapers.AnyAsync(x => x.UserID == newspaperID, cancellationToken)))
            return Result.Failure<PaginatedList<ArticleBreifResponse>>(NewspaperErrors.NotFoundNewspaper);

        var articleOfNewspaperKey = $"{prefixCacheVisitor}/{nameof(ArticleByNewspaper)}/{newspaperID}/{isPosted}";
        var cache = await hybridCache.GetOrCreateAsync(
            articleOfNewspaperKey,
            async entry =>
            {

                var articles = (await appDbContext.Articles.AsNoTracking()
                    .Where(x => x.NewsPaperID == newspaperID && (isPosted ? x.IsPosted : true))
                    .OrderByDescending(x => x.PostDate)
                        .ThenBy(x => x.Title)
                        .ThenByDescending(x => x.Views)
                        .ToListAsync(cancellationToken));
                var result = new List<ArticleBreifResponse>();
                foreach (var article in articles)
                    result.Add(await article.ToArticleBreifResponse(appDbContext, mapper, cancellationToken));
                return result;
            },
            cancellationToken: cancellationToken,
            tags: ["articles_of_newspaper", $"articles_of_newspaper_{newspaperID}"]
        );
        var result = PaginatedList<ArticleBreifResponse>.Create(cache, resultFilter.PageNumber, resultFilter.PageSize);
        return Result.Success(result);
    }

    //V16
    public async Task<Result<PaginatedList<CategoryResponse>>> GetAllCategories
        (ResultFilter resultFilter, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{prefixCacheVisitor}/{nameof(GetAllCategories)}";
        var data = await hybridCache.GetOrCreateAsync(
            cacheKey,
            async entry =>
            {
                var data = (await
                appDbContext.Categories
                    .OrderBy(x => x.CategoryName)
                    .ThenByDescending(x => x.Date)
                    .ToListAsync(cancellationToken))
                    .ToCategoriesRespones(mapper)
                    .ToList();
                return data;
            },
            tags: ["all_categories"]
            );
        var result = PaginatedList<CategoryResponse>.Create(data, resultFilter.PageNumber, resultFilter.PageSize);
        return Result.Success(result);
    }

    //V17
    public async Task<Result<(string imagePath, string contentType)>> GetProfileImage
        (string userID, UserTypes userTypes, bool isActive, CancellationToken cancellationToken = default)
    {
        if (!(await appDbContext.Users.AnyAsync(x => x.Id == userID && x.TypeUser == userTypes.ToString())))
            return Result.Failure<(string, string)>(UserErrors.NotFoundUser);

        if (isActive)
            if (!(await HelpToolsExtensions.checkUserActive(userID, userTypes, appDbContext, cancellationToken)))
                return Result.Failure<(string, string)>(userTypes == UserTypes.Member ? MemberErrors.NotActivatedMember : NewspaperErrors.NotActicatedNewspaper);

        var profileImage = await appDbContext.ProfileImages
            .SingleOrDefaultAsync(x => x.userID == userID, cancellationToken);
        if (profileImage is null)
            return Result.Failure<(string, string)>(ImageErrors.NotFoundImage);
        if (!File.Exists($"{profileImagesOptions.Path}\\{profileImage!.Name}"))
            return Result.Failure<(string, string)>(ImageErrors.NotFoundImage);

        var imageKey = $"{prefixCacheVisitor}/{nameof(GetProfileImage)}/{userID}/{isActive}";
        var cache = await hybridCache.GetOrCreateAsync(
            imageKey,
            async entry =>
            {
                return await Task.Run(() =>
                {
                    var env = webHostEnvironment.WebRootPath;
                    env = env.Replace("wwwroot", "");
                    var imagePath = Path.Combine(profileImagesOptions.Path, profileImage.Name);
                    var contentType = ImageHelper.GetContentType(imagePath);
                    var fullImagePath = Path.Combine(env, imagePath);
                    return (fullImagePath, contentType);
                });
            },
            cancellationToken: cancellationToken,
            tags: ["get_profile_image"]
        );

        return Result.Success((cache.fullImagePath, cache.contentType));
    }

    //V18
    public async Task<Result<(string imagePath, string contentType)>> GetArticleImage
        (string name, bool isPosted, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.Images.SingleOrDefaultAsync(x => x.Name == name, cancellationToken) is not { } image)
            return Result.Failure<(string ,string)>(ImageErrors.NotFoundImage);

        if (!(await appDbContext.Articles.AnyAsync(x => x.Id == image.ArticleID && isPosted ? x.IsPosted : true)))
            return Result.Failure<(string, string)>(ArticleErrors.NotFoundArticle);

        if (!File.Exists($"{articleImageOptions.Path}\\{image.Name}"))
            return Result.Failure<(string, string)>(ImageErrors.NotFoundImage);

        var imageKey = $"{prefixCacheVisitor}/{nameof(GetArticleImage)}/{name}/{isPosted}";
        var cache = await hybridCache.GetOrCreateAsync(
            imageKey,
            async entry =>
            {
                return await Task.Run(() =>
                {
                    var env = webHostEnvironment.WebRootPath;
                    env = env.Replace("wwwroot", "");
                    var imagePath = Path.Combine(articleImageOptions.Path, image.Name);
                    var contentType = ImageHelper.GetContentType(imagePath);
                    var fullImagePath = Path.Combine(env, imagePath);
                    return (fullImagePath, contentType);
                });
            },
            cancellationToken: cancellationToken,
            tags: ["get_article_image", $"get_article_image_{image.ArticleID}"]
        );

        return Result.Success((cache.fullImagePath, cache.contentType));
    }


    private async Task<List<ArticleBreifResponse>> GetArticles(bool isPosted, string orderBy, CancellationToken cancellationToken = default)
    {
        var articles = await appDbContext.Articles.AsNoTracking()
                .Where(x => isPosted ? x.IsPosted : true)
                .OrderBy($"{orderBy} DESC")
                .ToListAsync(cancellationToken);
        var result = new List<ArticleBreifResponse>();
        foreach (var article in articles)
            result.Add(await article.ToArticleBreifResponse(appDbContext, mapper, cancellationToken));
        return result;
    }
}

