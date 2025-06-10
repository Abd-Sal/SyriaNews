namespace SyriaNews.Repository.Implementations;

public class MemberService(
    AppDbContext appDbContext,
    IVisitorService visitorService,
    IOptions<ProfileImages> profileOptions,
    IMapper mapper,
    HybridCache hybridCache
    )
    : IMemberService
{
    private readonly AppDbContext appDbContext = appDbContext;
    private readonly IVisitorService visitorService = visitorService;
    private readonly ProfileImages profileOptions = profileOptions.Value;
    private readonly IMapper mapper = mapper;
    private readonly HybridCache hybridCache = hybridCache;
    private readonly string prefixMemberCache = nameof(prefixMemberCache);

    //M1
    public async Task<Result<PaginatedList<FullSaveResponse>>> FullSavedArtciels
        (string memberID, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default)
    {
        if (!(await appDbContext.Members.AnyAsync(x => x.UserID == memberID, cancellationToken)))
            return Result.Failure<PaginatedList<FullSaveResponse>>(MemberErrors.NotFoundMember);

        var savedKey = $"{prefixMemberCache}/{nameof(FullSavedArtciels)}/{memberID}";
        var allSavedArticles = await hybridCache.GetOrCreateAsync(
            savedKey,
            async entry =>
            {
                var saves = await appDbContext.Saves.AsNoTracking()
                    .Include(x => x.Article)
                    .Where(x => x.MemberID == memberID && (isPosted ? x.Article.IsPosted : true))
                    .OrderByDescending(x => x.Date)
                    .ToListAsync(cancellationToken);

                var articles = await appDbContext.Articles.Where(x => saves.Select(x => x.ArticleID).Contains(x.Id))
                    .ToListAsync(cancellationToken);

                var articleBreifResponse = new List<ArticleBreifResponse>();
                foreach (var article in articles)
                    articleBreifResponse.Add(await article.ToArticleBreifResponse(appDbContext, mapper, cancellationToken));

                var data = saves
                        .Select(x => new FullSaveResponse(x.Id, x.MemberID, x.Date, articleBreifResponse.First(a => a.Id == x.ArticleID)))
                        .ToList();
                return data;
            },
            cancellationToken: cancellationToken,
            tags: ["saved_articles"]
        );

        return Result.Success(PaginatedList<FullSaveResponse>.Create
               (allSavedArticles, resultFilter.PageNumber, resultFilter.PageSize));
    }

    //M2
    public async Task<Result<PaginatedList<ArticleBreifResponse>>> LikedArticle
        (string memberID, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default)
    {
        if (!(await appDbContext.Members.AnyAsync(x => x.UserID == memberID, cancellationToken)))
            return Result.Failure<PaginatedList<ArticleBreifResponse>>(MemberErrors.NotFoundMember);

        var likedKey = $"{prefixMemberCache}/{nameof(LikedArticle)}/{memberID}";
        var cache = await hybridCache.GetOrCreateAsync(
            likedKey,
            async entry =>
                {
                    var articles = await appDbContext.Likes.AsNoTracking()
                        .Include(x => x.Article)
                        .Where(x => x.MemberID == memberID)
                        .Select(x => x.Article)
                        .ToListAsync(cancellationToken);

                    var result = new List<ArticleBreifResponse>();
                    foreach (var article in articles)
                        result.Add(await article.ToArticleBreifResponse(appDbContext, mapper, cancellationToken));

                    return result;
                },
            cancellationToken: cancellationToken,
            tags: ["liked_articles"]
            );

        var result = PaginatedList<ArticleBreifResponse>.Create(cache, resultFilter.PageNumber, resultFilter.PageSize);
        return Result.Success(result);
    }

    //M3
    public async Task<Result<PaginatedList<NewspaperUserResponse>>> FollowedNewspapers
        (string memberID, ResultFilter resultFilter, CancellationToken cancellationToken = default)
    {
        if (!(await appDbContext.Members.AnyAsync(x => x.UserID == memberID, cancellationToken)))
            return Result.Failure<PaginatedList<NewspaperUserResponse>>(MemberErrors.NotFoundMember);

        var followedKey = $"{prefixMemberCache}/{nameof(FollowedNewspapers)}/{memberID}";
        var cache = await hybridCache.GetOrCreateAsync(
            followedKey,
            async entry =>
            {
                var data = await 
                    appDbContext.Followers.AsNoTracking()
                    .Where(x => x.MemberID == memberID)
                    .OrderByDescending(x => x.Date)
                    .Join(appDbContext.NewsPapers.AsNoTracking(),
                        f => f.NewsPaperID,
                        n => n.UserID,
                        (f, n) => n
                    )
                    .Join(appDbContext.Users.AsNoTracking().Include(x => x.ProfileImage).Select(x => new { Id = x.Id, Email = x.Email, JoinAt = x.JoinAt, TypeUser = x.TypeUser, Profile = x.ProfileImage.ToProfileImageResponse(mapper) }),
                        newspaper => newspaper.UserID,
                        user => user.Id,
                        (newspaper, user) => new NewspaperUserResponse(
                            newspaper.UserID, user.Email!, newspaper.Name, user.TypeUser, user.JoinAt,
                            newspaper.followers, newspaper.IsActive, user.Profile
                        )
                    ).ToListAsync(cancellationToken);
                return data;
            },
            cancellationToken: cancellationToken,
            tags: ["followed_newspaper"]
        );
        var result = PaginatedList<NewspaperUserResponse>.Create(cache, resultFilter.PageNumber, resultFilter.PageSize);
        return Result.Success(result);
    }

    //M4
    public async Task<Result<FullCommentResponse>> CommentOnArticle
        (string memberID, CommentRequest commentRequest, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.Members.AnyAsync(x => x.UserID == memberID && !x.IsActive, cancellationToken))
            return Result.Failure<FullCommentResponse>(MemberErrors.NotActivatedMember);
        
        if(await appDbContext.Articles.SingleOrDefaultAsync(x => x.Id == commentRequest.ArticleID && x.IsPosted, cancellationToken) is not { } article)
            return Result.Failure<FullCommentResponse>(ArticleErrors.NotFoundArticle);

        var member = await appDbContext.Members.FindAsync(memberID, cancellationToken);

        var simpleNotificationMessage = new SimpleNotificationRequest(article.Id,
            ConstantStrings.MemberCommentOnArticle($"{member!.FirstName} {member.LastName}").Title,
            ConstantStrings.MemberCommentOnArticle($"{member!.FirstName} {member.LastName}").Message);

        var comment = commentRequest.ToComment(mapper);
        comment.MemberID = memberID;
        var result = await appDbContext.Comments.AddAsync(comment, cancellationToken);
        var allComments = ++article.AllComments;
        var updates = await appDbContext.Articles.Where(x => x.Id == comment.ArticleID)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.AllComments, allComments),
                cancellationToken
            );
        await appDbContext.SaveChangesAsync(cancellationToken);
        var fullArticleBreif = await article.ToArticleBreifResponse(appDbContext, mapper, cancellationToken);
        var fullComent = new FullCommentResponse(comment.Id, comment.Content, 
            fullArticleBreif, (await appDbContext.Members.SingleAsync(x => x.UserID == memberID, cancellationToken)).ToMemberBreifResponse(mapper),
            comment.CreateDate);

        await NotificationNewspaperForNewComment(memberID, article.NewsPaperID, simpleNotificationMessage, cancellationToken); ;

        await hybridCache.RemoveAsync($"{prefixMemberCache}/{nameof(MemberComments)}/{memberID}", cancellationToken);
        await hybridCache.RemoveByTagAsync($"comments_for_article_{comment.ArticleID}");
        await hybridCache.RemoveByTagAsync($"show_member_likes_{comment.ArticleID}");
        await hybridCache.RemoveByTagAsync($"get_all_notifications_for_newspaper_{article.NewsPaperID}");

        return Result.Success(fullComent);
    }

    //M5
    public async Task<Result<FullLikeResponse>> LikeArticle
        (string memberID, LikeRequest likeRequest, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.Members.AnyAsync(x => x.UserID == memberID && !x.IsActive, cancellationToken))
            return Result.Failure<FullLikeResponse>(MemberErrors.NotActivatedMember);

        if (await appDbContext.Articles.SingleOrDefaultAsync(x => x.Id == likeRequest.ArticleID && x.IsPosted, cancellationToken) is not { } article)
            return Result.Failure<FullLikeResponse>(ArticleErrors.NotFoundArticle);

        if (await appDbContext.Likes.AnyAsync(x => x.ArticleID == likeRequest.ArticleID && x.MemberID == memberID, cancellationToken))
            return Result.Failure<FullLikeResponse>(LikeErrors.DuplicatedLike);

        var member = await appDbContext.Members.FindAsync(memberID, cancellationToken);

        var simpleNotificationMessage = new SimpleNotificationRequest(article.Id,
            ConstantStrings.MemberLikeArticle($"{member!.FirstName} {member.LastName}").Title,
            ConstantStrings.MemberLikeArticle($"{member!.FirstName} {member.LastName}").Message);

        var like = likeRequest.ToLike(mapper);
        like.MemberID = memberID;
        var result = await appDbContext.Likes.AddAsync(like, cancellationToken);
        var updates = await appDbContext.Articles.Where(x => x.Id == like.ArticleID)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.AllLikes, (article.AllLikes + 1)),
                cancellationToken
            );
        await appDbContext.SaveChangesAsync(cancellationToken);
        var fullArticle = await article.ToArticleBreifResponse(appDbContext, mapper, cancellationToken);
        var memberBreif = (await appDbContext.Members.SingleAsync(x => x.UserID == memberID, cancellationToken)).ToMemberBreifResponse(mapper);
        var fullLike = new FullLikeResponse(like.Id, memberBreif, fullArticle, like.Date);

        var notificationNewspaper = await NotificationNewspaperForNewLike(memberID, article.NewsPaperID, simpleNotificationMessage, cancellationToken);

        await hybridCache.RemoveAsync($"{prefixMemberCache}/{nameof(LikedArticle)}/{memberID}", cancellationToken);
        await hybridCache.RemoveAsync($"{prefixMemberCache}/{nameof(IsLiked)}/{memberID}/{likeRequest.ArticleID}", cancellationToken);
        await hybridCache.RemoveByTagAsync("count_likes_by_categories");
        await hybridCache.RemoveByTagAsync("count_views_by_categories");
        await hybridCache.RemoveByTagAsync("show_articles");
        await hybridCache.RemoveByTagAsync("member_comments");
        await hybridCache.RemoveByTagAsync("saved_articles");
        await hybridCache.RemoveByTagAsync($"read_{article.Id}");
        await hybridCache.RemoveByTagAsync($"count_of_views_{article.NewsPaperID}");
        await hybridCache.RemoveByTagAsync($"count_of_likes_{article.NewsPaperID}");
        await hybridCache.RemoveByTagAsync($"articles_of_newspaper_{article.NewsPaperID}");
        await hybridCache.RemoveByTagAsync($"show_member_likes_{article.NewsPaperID}");
        await hybridCache.RemoveByTagAsync($"get_all_notifications_for_newspaper_{article.NewsPaperID}");

        return Result.Success(fullLike);
    }

    //M6
    public async Task<Result<FullFollowerResponse>> FollowingNewspaper
        (string memberID, FollowerRequest followerRequest, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.Members.AnyAsync(x => x.UserID == memberID && !x.IsActive, cancellationToken))
            return Result.Failure<FullFollowerResponse>(MemberErrors.NotActivatedMember);

        if (await appDbContext.NewsPapers.SingleOrDefaultAsync(x => x.UserID == followerRequest.NewsPaperID && x.IsActive, cancellationToken) is not { } newspaper)
            return Result.Failure<FullFollowerResponse>(NewspaperErrors.NotFoundNewspaper);

        if (await appDbContext.Followers.AnyAsync(x => x.MemberID == memberID && x.NewsPaperID == followerRequest.NewsPaperID, cancellationToken))
            return Result.Failure<FullFollowerResponse>(FollowerErrors.DuplicatedFollower);

        var member = await appDbContext.Members.FindAsync(memberID, cancellationToken);

        var simpleNotificationMessage = new SimpleNotificationRequest(memberID,
            ConstantStrings.MemberFollowNewspaper($"{member!.FirstName} {member.LastName}").Title,
            ConstantStrings.MemberFollowNewspaper($"{member!.FirstName} {member.LastName}").Message);

        var follow = followerRequest.ToFollower(mapper);
        follow.MemberID = memberID;
        var result = await appDbContext.Followers.AddAsync(follow, cancellationToken);
        var updates = await appDbContext.NewsPapers.Where(x => x.UserID == follow.NewsPaperID)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.followers, (newspaper.followers + 1)),
                    cancellationToken
            );
        await appDbContext.SaveChangesAsync(cancellationToken);
        var memberBreif = (await appDbContext.Members.SingleAsync(x => x.UserID == memberID, cancellationToken)).ToMemberBreifResponse(mapper);
        var newspaperBreif = (await appDbContext.NewsPapers.SingleAsync(x => x.UserID == follow.NewsPaperID, cancellationToken)).ToNewspaperBreifResponse(mapper);
        var fullFollow = new FullFollowerResponse(follow.Id, newspaperBreif, memberBreif, follow.Date);

        var notificationNewspaper = await NotificationNewspaperForNewFollower(memberID, followerRequest.NewsPaperID, simpleNotificationMessage, cancellationToken);

        await hybridCache.RemoveAsync($"{prefixMemberCache}/{nameof(IsFollowed)}/{memberID}/{followerRequest.NewsPaperID}");
        await hybridCache.RemoveAsync($"prefixCacheVisitor/{nameof(ShowMemberFollowing)}/{followerRequest.NewsPaperID}/{true}");
        await hybridCache.RemoveAsync($"prefixCacheVisitor/{nameof(ShowMemberFollowing)}/{followerRequest.NewsPaperID}/{false}");
        await hybridCache.RemoveAsync($"{prefixMemberCache}/{nameof(FollowedNewspapers)}/{memberID}");
        await hybridCache.RemoveByTagAsync($"get_all_notifications_for_newspaper_{followerRequest.NewsPaperID}");

        return Result.Success(fullFollow);
    }

    //M7
    public async Task<Result> UpdateComment
        (string memberID, string commentID, CommentUpdateRequest commentUpdateRequest, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.Members.AnyAsync(x => x.UserID == memberID && !x.IsActive, cancellationToken))
            return Result.Failure(MemberErrors.NotActivatedMember);

        if (await appDbContext.Comments.FindAsync(commentID, cancellationToken) is not { } comment)
            return Result.Failure(CommentErrors.NotFoundComment);

        if (!(await appDbContext.Articles.AnyAsync(x => x.Id == comment.ArticleID && x.IsPosted, cancellationToken)))
            return Result.Failure<CommentResponse>(ArticleErrors.NotFoundArticle);

        var result = await appDbContext.Comments.Where(x => x.Id == commentID)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.Content, commentUpdateRequest.Content)
                    .SetProperty(x => x.LastUpdate, DateTime.UtcNow),
                cancellationToken
            );

        await hybridCache.RemoveAsync($"{prefixMemberCache}/{nameof(MemberComments)}/{memberID}", cancellationToken);
        await hybridCache.RemoveByTagAsync($"comments_for_article_{comment.ArticleID}");
        await hybridCache.RemoveByTagAsync($"show_member_likes_{comment.ArticleID}");

        return Result.Success();
    }

    //M8
    public async Task<Result> RemoveComment
        (string memberID, string commentID, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.Members.AnyAsync(x => x.UserID == memberID && !x.IsActive, cancellationToken))
            return Result.Failure(MemberErrors.NotActivatedMember);

        if (!(await appDbContext.Comments.AnyAsync(x => x.Id == commentID, cancellationToken)))
            return Result.Failure(CommentErrors.NotFoundComment);

        var article = (await appDbContext.Comments.Include(x => x.Article).SingleAsync(x => x.Id == commentID, cancellationToken))
        .Article;

        if (!article.IsPosted)
            return Result.Failure(ArticleErrors.NotFoundArticle);

        var update = await appDbContext.Articles.Where(x => x.Id == article.Id)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.AllComments, (article.AllComments - 1))
            );

        var result = await appDbContext.Comments.Where(x => x.Id == commentID)
            .ExecuteDeleteAsync(cancellationToken);

        await hybridCache.RemoveAsync($"{prefixMemberCache}/{nameof(MemberComments)}/{memberID}", cancellationToken);
        await hybridCache.RemoveByTagAsync($"comments_for_article_{article.Id}");
        await hybridCache.RemoveByTagAsync($"show_member_likes_{article.Id}");

        return Result.Success();
    }

    //M9
    public async Task<Result> RemoveLike
        (string memberID, string articleID, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.Members.AnyAsync(x => x.UserID == memberID && !x.IsActive, cancellationToken))
            return Result.Failure(MemberErrors.NotActivatedMember);

        if (!(await appDbContext.Likes.AnyAsync(x => x.ArticleID == articleID && x.MemberID == memberID, cancellationToken)))
            return Result.Failure(LikeErrors.NotFoundLike);

        var article = (await appDbContext.Likes
            .Include(x => x.Article).SingleAsync(x => x.ArticleID == articleID && x.MemberID == memberID, cancellationToken))
            .Article;

        if (!article.IsPosted)
            return Result.Failure(ArticleErrors.NotFoundArticle);

        var updates = await appDbContext.Articles.Where(x => x.Id == article.Id)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.AllLikes, (article.AllLikes - 1))
            );

        var result = await appDbContext.Likes.Where(x => x.ArticleID == articleID && x.MemberID == memberID)
            .ExecuteDeleteAsync(cancellationToken);

        await hybridCache.RemoveAsync($"{prefixMemberCache}/{nameof(LikedArticle)}/{memberID}", cancellationToken);
        await hybridCache.RemoveAsync($"{prefixMemberCache}/{nameof(IsLiked)}/{memberID}/{articleID}", cancellationToken);
        await hybridCache.RemoveByTagAsync("count_likes_by_categories");
        await hybridCache.RemoveByTagAsync("count_views_by_categories");
        await hybridCache.RemoveByTagAsync("show_articles");
        await hybridCache.RemoveByTagAsync("member_comments");
        await hybridCache.RemoveByTagAsync("saved_articles");
        await hybridCache.RemoveByTagAsync($"read_{article.Id}");
        await hybridCache.RemoveByTagAsync($"count_of_views_{article.NewsPaperID}");
        await hybridCache.RemoveByTagAsync($"count_of_likes_{article.NewsPaperID}");
        await hybridCache.RemoveByTagAsync($"articles_of_newspaper_{article.NewsPaperID}");
        await hybridCache.RemoveByTagAsync($"show_member_likes_{article.NewsPaperID}");

        return Result.Success();
    }

    //M10
    public async Task<Result> RemoveFollow
        (string memberID, string newspaperID, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.Members.AnyAsync(x => x.UserID == memberID && !x.IsActive, cancellationToken))
            return Result.Failure(MemberErrors.NotActivatedMember);

        if (!(await appDbContext.Followers.AnyAsync(x => x.NewsPaperID == newspaperID && x.MemberID == memberID, cancellationToken)))
            return Result.Failure(FollowerErrors.NotFoundFollower);

        var newspaper = (await appDbContext.Followers.Include(x => x.NewsPaper)
            .SingleAsync(x => x.NewsPaperID == newspaperID && x.MemberID == memberID, cancellationToken)).NewsPaper;

        var updates = await appDbContext.NewsPapers.Where(x => x.UserID == newspaper.UserID)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.followers, (newspaper.followers - 1))
            );

        var result = await appDbContext.Followers.Where(x => x.NewsPaperID == newspaperID && x.MemberID == memberID)
            .ExecuteDeleteAsync(cancellationToken);

        await hybridCache.RemoveAsync($"{prefixMemberCache}/{nameof(FollowedNewspapers)}/{memberID}", cancellationToken);
        await hybridCache.RemoveAsync($"{prefixMemberCache}/{nameof(IsFollowed)}/{memberID}/{newspaperID}");

        await hybridCache.RemoveAsync($"{prefixMemberCache}/{nameof(IsFollowed)}/{memberID}/{newspaperID}");
        await hybridCache.RemoveAsync($"prefixCacheVisitor/{nameof(ShowMemberFollowing)}/{newspaperID}/{true}");
        await hybridCache.RemoveAsync($"prefixCacheVisitor/{nameof(ShowMemberFollowing)}/{newspaperID}/{false}");
        await hybridCache.RemoveAsync($"{prefixMemberCache}/{nameof(FollowedNewspapers)}/{memberID}");

        return Result.Success();
    }

    //M11
    public async Task<Result<SaveResponse>> SaveArticle
        (string memberID, SaveRequest saveRequest, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.Members.AnyAsync(x => x.UserID == memberID && !x.IsActive, cancellationToken))
            return Result.Failure<SaveResponse>(MemberErrors.NotActivatedMember);

        if (!(await appDbContext.Articles.AnyAsync(x => x.Id == saveRequest.ArticleID && x.IsPosted, cancellationToken)))
            return Result.Failure<SaveResponse>(ArticleErrors.NotFoundArticle);

        if (await appDbContext.Saves.AnyAsync(x => x.MemberID == memberID && x.ArticleID == saveRequest.ArticleID, cancellationToken))
            return Result.Failure<SaveResponse>(SaveErrors.DuplicatedSave);

        var save = saveRequest.ToSave(mapper);
        save.MemberID = memberID;
        var result = await appDbContext.Saves.AddAsync(save, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);

        await hybridCache.RemoveAsync($"{prefixMemberCache}/{nameof(FullSavedArtciels)}/{memberID}", cancellationToken);
        await hybridCache.RemoveAsync($"{prefixMemberCache}/{nameof(IsSaved)}/{memberID}/{saveRequest.ArticleID}", cancellationToken);

        return Result.Success(save.ToSaveResponse(mapper));
    }

    //M12
    public async Task<Result> UnsaveArticle
        (string memberID, string articleID, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.Members.AnyAsync(x => x.UserID == memberID && !x.IsActive, cancellationToken))
            return Result.Failure(MemberErrors.NotActivatedMember);

        if (!(await appDbContext.Saves.AnyAsync(x => x.ArticleID == articleID && x.MemberID == memberID, cancellationToken)))
            return Result.Failure(SaveErrors.NotFoundSave);

        var result = await appDbContext.Saves.Where(x => x.ArticleID == articleID && x.MemberID == memberID)
            .ExecuteDeleteAsync(cancellationToken);

        await hybridCache.RemoveAsync($"{prefixMemberCache}/{nameof(FullSavedArtciels)}/{memberID}", cancellationToken);
        await hybridCache.RemoveAsync($"{prefixMemberCache}/{nameof(IsSaved)}/{memberID}/{articleID}", cancellationToken);

        return Result.Success();
    }

    //M13
    public async Task<Result> UpdateMemberProfile
        (string memberID, MemberRequest memberRequest, CancellationToken cancellationToken = default)
    {
        var member = await appDbContext.Members.SingleAsync(x => x.UserID == memberID, cancellationToken);
        if (!member.IsActive)
            return Result.Failure(MemberErrors.NotActivatedMember);

        var temp = await appDbContext.Members.Where(x => x.UserID == memberID)
            .ExecuteUpdateAsync(setters =>
                setters
                .SetProperty(x => x.FirstName, memberRequest.FirstName)
                .SetProperty(x => x.LastName, memberRequest.LastName)
                .SetProperty(x => x.Gender, memberRequest.Gender)
            );

        return Result.Success();
    }

    //M14
    public async Task<Result<PaginatedList<FullCommentResponse>>> MemberComments
        (string memberID, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default)
    {
        if (!(await appDbContext.Members.AnyAsync(x => x.UserID == memberID, cancellationToken)))
            return Result.Failure<PaginatedList<FullCommentResponse>>(MemberErrors.NotFoundMember);

        var commentKey = $"{prefixMemberCache}/{nameof(MemberComments)}/{memberID}";
        var cache = await hybridCache.GetOrCreateAsync(
            commentKey,
            async entry =>
                {
                    var temp = await appDbContext.Comments.AsNoTracking().Include(x => x.Article)
                        .Where(x => x.MemberID == memberID && isPosted ? x.Article.IsPosted : true)
                        .Select(x => new { Comment = x, Article = x.Article})
                        .ToListAsync(cancellationToken);

                    var memberBreif = (await appDbContext.Members.AsNoTracking().SingleAsync(x => x.UserID == memberID, cancellationToken))
                        .ToMemberBreifResponse(mapper);

                    var articleBriefResponse = new HashSet<ArticleBreifResponse>();
                    foreach (var article in temp)
                        articleBriefResponse.Add(await article.Article.ToArticleBreifResponse(appDbContext, mapper, cancellationToken));

                    var fullComments = temp.Select(x => new FullCommentResponse(
                            x.Comment.Id,
                            x.Comment.Content,
                            articleBriefResponse.First(a => a.Id == x.Article.Id),
                            memberBreif,
                            x.Comment.CreateDate
                            )).ToList();

                    return fullComments;
                },
            cancellationToken: cancellationToken,
            tags: ["member_comments"]
            );
        var result = PaginatedList<FullCommentResponse>.Create(cache, resultFilter.PageNumber, resultFilter.PageSize);
        return Result.Success(result);
    }

    //M15
    public async Task<Result<ProfileImageResponse>> ChangeProfileImage
        (string memberID, IFormFile formFile, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.Members.AnyAsync(x => x.UserID == memberID && !x.IsActive, cancellationToken))
            return Result.Failure<ProfileImageResponse>(MemberErrors.NotActivatedMember);

        if (formFile is null)
            return Result.Failure<ProfileImageResponse>(ImageErrors.NullImage);

        var imageCheck = await appDbContext.ProfileImages.FirstOrDefaultAsync(x => x.userID == memberID);
        if (imageCheck is not null)
        {
            await appDbContext.ProfileImages.Where(x => x.userID == memberID)
                .ExecuteDeleteAsync(cancellationToken);
            ImageHelper.Delete($"{profileOptions.Path}\\{imageCheck.Name}");
        }

        var saveImage = await ImageHelper.Save(formFile, profileOptions);
        if (saveImage.IsFailure)
            return Result.Failure<ProfileImageResponse>(saveImage.Error);

        var image = new ProfileImage
        {
            Name = saveImage.Value.imageName,
            Date = DateTime.UtcNow,
            userID = memberID,
        };
        var addedImage = await appDbContext.ProfileImages.AddAsync(image, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(addedImage.Entity.ToProfileImageResponse(mapper)!);
    }

    //M16
    public async Task<Result> RemoveProfileImage
        (string memberID, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.Members.AnyAsync(x => x.UserID == memberID && !x.IsActive, cancellationToken))
            return Result.Failure(MemberErrors.NotActivatedMember);

        var image = await appDbContext.ProfileImages.SingleOrDefaultAsync(x => x.userID == memberID, cancellationToken);
        if (image is null)
            return Result.Success();

        var deleteImage = ImageHelper.Delete($"{profileOptions.Path}\\{image.Name}");
        if (deleteImage.IsFailure)
            return Result.Failure(deleteImage.Error);
        var result = await appDbContext.ProfileImages.Where(x => x.userID == memberID)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////Visistor Services//////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    //M17
    public async Task<Result<PaginatedList<ArticleBreifResponse>>> ShowNewArticlesByPostDate
        (ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await visitorService.ShowNewArticlesByPostDate(true, resultFilter, cancellationToken);

    //M18
    public async Task<Result<PaginatedList<ArticleBreifResponse>>> ShowNewArticlesByMostViewed
        (ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await visitorService.ShowNewArticlesByMostViewed(true, resultFilter, cancellationToken);

    //M19
    public async Task<Result<PaginatedList<ArticleBreifResponse>>> ShowNewArticlesByTag
        (string tagName, ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await visitorService.ShowNewArticlesByTag(tagName, true, resultFilter, cancellationToken);

    //M20
    public async Task<Result<PaginatedList<ArticleBreifResponse>>> ShowNewArticlesByCategory
        (string categoryID, ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await visitorService.ShowNewArticlesByCategory(categoryID, true, resultFilter, cancellationToken);

    //M21
    public async Task<Result<PaginatedList<ArticleBreifResponse>>> ShowNewArticlesByTitle
        (string title, ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await visitorService.ShowNewArticlesByTitle(title, true, resultFilter, cancellationToken);

    //M22
    public async Task<Result<PaginatedList<NewspaperUserResponse>>> SearchForNewspaper
        (string newspaperName, ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await visitorService.SearchForNewspaper(newspaperName, true, resultFilter, cancellationToken);

    //M23
    public async Task<Result<FullArticleFullResponse>> ReadArticle
        (string? memberID, string articleID, CancellationToken cancellationToken = default)
        =>await visitorService.ReadArticle(memberID, articleID, true, cancellationToken);
  
    //M24
    public async Task<Result<PaginatedList<CommentViewResponse>>> CommentsForArticle
        (string articleID, ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await visitorService.CommentsForArticle(articleID, true, resultFilter, cancellationToken);

    //M25
    public async Task<Result<NewspaperUserResponse>> NewspaperProfile
        (string newspaperID, CancellationToken cancellationToken = default)
        => await visitorService.NewspaperProfile(newspaperID, true, cancellationToken);

    //M26
    public async Task<Result<MemberUserResponse>> MemberProfile
        (string memberID, CancellationToken cancellationToken = default)
        => await visitorService.MemberProfile(memberID, true, cancellationToken);

    //M27
    public async Task<Result<MemberUserResponse>> MyProfile
        (string memberID, CancellationToken cancellationToken = default)
        => await visitorService.MemberProfile(memberID, false, cancellationToken);

    //M28
    public async Task<Result<PaginatedList<MemberUserResponse>>> ShowMembersLikes
        (string articleID, ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await visitorService.ShowMembersLikes(null, articleID, true, resultFilter, cancellationToken);

    //M29
    public async Task<Result<PaginatedList<MemberBreifResponseWithProfileImage>>> ShowMemberFollowing
        (string newspaperID, ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await visitorService.ShowMemberFollowing(newspaperID, true, resultFilter, cancellationToken);

    //M30
    public async Task<Result<PaginatedList<ArticleBreifResponse>>> ArticleByNewspaper
        (string newspaperID, ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await visitorService.ArticleByNewspaper(newspaperID, true, resultFilter, cancellationToken);

    //M31
    public async Task<Result<PaginatedList<CategoryResponse>>> GetAllCategories
        (ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await visitorService.GetAllCategories(resultFilter, cancellationToken);

    //M32
    public async Task<Result<(string imagePath, string contentType)>> GetMyProfileImage
        (string membrID, CancellationToken cancellationToken = default)
        => await visitorService.GetProfileImage(membrID, UserTypes.Member, false, cancellationToken);

    //M33
    public async Task<Result<(string imagePath, string contentType)>> GetMemberProfileImage
        (string membrID, CancellationToken cancellationToken = default)
        => await visitorService.GetProfileImage(membrID, UserTypes.Member, true, cancellationToken);

    //M34
    public async Task<Result<(string imagePath, string contentType)>> GetNewspaperProfileImage
        (string membrID, CancellationToken cancellationToken = default)
        => await visitorService.GetProfileImage(membrID, UserTypes.NewsPaper, true, cancellationToken);

    //M35
    public async Task<Result<(string imagePath, string contentType)>> GetArticleImage
        (string imageName, CancellationToken cancellationToken)
        => await visitorService.GetArticleImage(imageName, true, cancellationToken);

    //M36
    public async Task<Result<bool>> IsLiked
        (string memberID, string articleID, CancellationToken cancellationToken = default)
    {
        if (!(await appDbContext.Articles.AnyAsync(x => x.Id == articleID && x.IsPosted, cancellationToken)))
            return Result.Failure<bool>(ArticleErrors.NotFoundArticle);

        var isLikedKey = $"{prefixMemberCache}/{nameof(IsLiked)}/{memberID}/{articleID}";
        var cache = await hybridCache.GetOrCreateAsync(
            isLikedKey,
            async entry =>
            {
                if (await appDbContext.Likes.AnyAsync(x => x.ArticleID == articleID && x.MemberID == memberID, cancellationToken))
                    return true;
                return false;
            },
            cancellationToken: cancellationToken
        );

        return Result.Success(cache);
    }

    //M37
    public async Task<Result<bool>> IsSaved
        (string memberID, string articleID, CancellationToken cancellationToken = default)
    {

        if (!(await appDbContext.Articles.AnyAsync(x => x.Id == articleID && x.IsPosted, cancellationToken)))
            return Result.Failure<bool>(ArticleErrors.NotFoundArticle);

        var isLikeKey = $"{prefixMemberCache}/{nameof(IsSaved)}/{memberID}/{articleID}";
        var cache = await hybridCache.GetOrCreateAsync(
            isLikeKey,
            async entry =>
            {
                if (await appDbContext.Saves.AnyAsync(x => x.ArticleID == articleID && x.MemberID == memberID, cancellationToken))
                    return true;
                return false;
            },
            cancellationToken: cancellationToken
        );

        return Result.Success(cache);
    }

    //M38
    public async Task<Result<bool>> IsFollowed
        (string memberID, string newspaperID, CancellationToken cancellationToken = default)
    {
        if(!(await appDbContext.NewsPapers.AnyAsync(x => x.UserID == newspaperID, cancellationToken)))
            return Result.Failure<bool>(NewspaperErrors.NotFoundNewspaper);

        if (!(await appDbContext.NewsPapers.AnyAsync(x => x.UserID == newspaperID && x.IsActive , cancellationToken)))
            return Result.Failure<bool>(NewspaperErrors.NotActicatedNewspaper);

        var isFollowedKey = $"{prefixMemberCache}/{nameof(IsFollowed)}/{memberID}/{newspaperID}";
        var cache = await hybridCache.GetOrCreateAsync(
            isFollowedKey,
            async entry => {
                    if (await appDbContext.Followers.AnyAsync(x => x.NewsPaperID == newspaperID && x.MemberID == memberID, cancellationToken))
                        return true;
                    return false;
                },
            cancellationToken: cancellationToken,
            tags: ["followed", $"follow_{memberID}_{newspaperID}"]
        );
        return Result.Success(cache);
    }

    //M39
    public async Task<Result> ReportForNewspaper
        (string memberID, ReportForNewspaperRequest reportForNewspaperRequest,CancellationToken cancellationToken = default)
    {
        if (await appDbContext.Members.AnyAsync(x => x.UserID == memberID && !x.IsActive, cancellationToken))
            return Result.Failure(MemberErrors.NotActivatedMember);

        if(!(await appDbContext.NewsPapers.AnyAsync(x => x.UserID == reportForNewspaperRequest.NewspaperID && x.IsActive, cancellationToken)))
            return Result.Failure(NewspaperErrors.NotFoundNewspaper);

        var notification = new Notification
        {
            NotificationTypes = NotificationTypes.ReportNotification.ToString(),
            Title = reportForNewspaperRequest.Title,
            Message = reportForNewspaperRequest.Message,
            EntityID = reportForNewspaperRequest.NewspaperID,
            EntityType = EntitiesTypes.User.ToString(),
            SenderUserID = memberID,
            ReceiveUserID = DefaultUsers.AdminID,
        };

        await appDbContext.AddAsync(notification, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    //M40
    public async Task<Result> ReportForArticle
        (string memberID, ReportForArticleRequest reportForArticleRequest, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.Members.AnyAsync(x => x.UserID == memberID && !x.IsActive, cancellationToken))
            return Result.Failure(MemberErrors.NotActivatedMember);

        if (await appDbContext.Articles.SingleOrDefaultAsync(x => x.Id == reportForArticleRequest.ArticleID && x.IsPosted, cancellationToken) is not { } article)
            return Result.Failure(ArticleErrors.NotFoundArticle);

        var notification = new Notification
        {
            NotificationTypes = NotificationTypes.ReportNotification.ToString(),
            Title = reportForArticleRequest.Title,
            Message = reportForArticleRequest.Message,
            EntityID = article.Id,
            EntityType = EntitiesTypes.Article.ToString(),
            SenderUserID = memberID,
            ReceiveUserID = DefaultUsers.AdminID,
        };

        await appDbContext.AddAsync(notification, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    //M41
    public async Task<Result<PaginatedList<NotificationResponse>>> GetAllNotification
        (string memberID, bool isReaded, ResultFilter resultFilter, CancellationToken cancellationToken = default)
    {
        var notificationKey = $"{prefixMemberCache}/{nameof(GetAllNotification)}/{memberID}/{isReaded}";
        var cache = await hybridCache.GetOrCreateAsync(
            notificationKey,
            async entry =>
            {
                var temp =
                    appDbContext.Notifications
                    .AsNoTracking()
                    .Where(x => x.ReceiveUserID == memberID && isReaded ? !x.IsReaded : true)
                    .OrderByDescending(x => x.CreatedAt).ThenBy(x => x.IsReaded);
                var data = await temp.Select(x =>
                        new NotificationResponse(x.Id, x.Title, x.Message, x.NotificationTypes, x.EntityID, x.EntityType, x.CreatedAt, x.IsReaded)
                    ).ToListAsync(cancellationToken);
                return data;
            },
            cancellationToken: cancellationToken,
            tags: ["get_all_notifications_for_member"]
        );
        var result = PaginatedList<NotificationResponse>.Create(cache, resultFilter.PageNumber, resultFilter.PageSize);
        return Result.Success(result);
    }
    
    //M42
    public async Task<Result> ReadAllNotifications
        (string memberID, CancellationToken cancellationToken = default)
    {
        var temp = await appDbContext.Notifications
            .Where(x => x.ReceiveUserID == memberID && !x.IsReaded)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.IsReaded, true)
            );

        await hybridCache.RemoveByTagAsync("get_all_notifications");
        await hybridCache.RemoveByTagAsync("count_of_notifications");
        return Result.Success();
    }

    //M43
    public async Task<Result> ReadNotification
        (string memberID, string notificationID, CancellationToken cancellationToken = default)
    {
        if (!(await appDbContext.Notifications.AnyAsync(x => x.Id == notificationID, cancellationToken)))
            return Result.Failure(NotificationErrors.NotificationNotFound);

        if (!(await appDbContext.Notifications.AnyAsync(x => x.ReceiveUserID == memberID && x.Id == notificationID, cancellationToken)))
            return Result.Failure(NotificationErrors.NotificationNotFound);

        await appDbContext.Notifications.Where(x => x.Id == notificationID)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.IsReaded, true)
            );
        await hybridCache.RemoveAsync($"{prefixMemberCache}/{nameof(GetAllNotification)}/{memberID}/{true}");
        await hybridCache.RemoveByTagAsync("get_all_notifications");
        await hybridCache.RemoveByTagAsync("count_of_notifications");
        return Result.Success();
    }

    //M44
    public async Task<Result<int>> CountOfNotifications
        (string memberID, bool isReaded, CancellationToken cancellationToken = default)
    {
        var countKey = $"{prefixMemberCache}/{CountOfNotifications}/{memberID}/{isReaded}";
        var cache = await hybridCache.GetOrCreateAsync(
            countKey,
            async entry =>
                await appDbContext.Notifications.AsNoTracking()
                    .CountAsync(x => x.ReceiveUserID == memberID && (isReaded ? !x.IsReaded : true)),
            cancellationToken: cancellationToken,
            tags: ["count_of_notifications_for_member"]
        );
        return Result.Success(cache);
    }
    
    private async Task<Result> NotificationNewspaperForNewComment
        (string memberID, string newspaperID, SimpleNotificationRequest simpleNotificationRequest, CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            NotificationTypes = NotificationTypes.CommentNotification.ToString(),
            Title = simpleNotificationRequest.Title,
            Message = simpleNotificationRequest.Message,
            SenderUserID = memberID,
            ReceiveUserID = newspaperID,
            EntityID = simpleNotificationRequest.EntityID,
            EntityType = EntitiesTypes.Article.ToString(),
        };

        await appDbContext.Notifications.AddAsync(notification, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<Result> NotificationNewspaperForNewLike
        (string memberID, string newspaperID, SimpleNotificationRequest simpleNotificationRequest, CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            NotificationTypes = NotificationTypes.LikeNotification.ToString(),
            Title = simpleNotificationRequest.Title,
            Message = simpleNotificationRequest.Message,
            SenderUserID = memberID,
            ReceiveUserID = newspaperID,
            EntityID = simpleNotificationRequest.EntityID,
            EntityType = EntitiesTypes.Article.ToString(),
        };

        await appDbContext.Notifications.AddAsync(notification, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<Result> NotificationNewspaperForNewFollower
        (string memberID, string newspaperID, SimpleNotificationRequest simpleNotificationRequest, CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            NotificationTypes = NotificationTypes.FollowNotification.ToString(),
            Title = simpleNotificationRequest.Title,
            Message = simpleNotificationRequest.Message,
            SenderUserID = memberID,
            ReceiveUserID = newspaperID,
            EntityID = simpleNotificationRequest.EntityID,
            EntityType = EntitiesTypes.User.ToString(),
        };

        await appDbContext.Notifications.AddAsync(notification, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

