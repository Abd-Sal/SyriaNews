namespace SyriaNews.Repository.Implementations;

public class NewsPaperService(
    AppDbContext appDbContext,
    IMapper _mapper,
    HybridCache hybridCache,
    IVisitorService visitorService,
    IOptions<ArticleImages> imagesOptions,
    IOptions<ProfileImages> profileOptions
    ) : INewsPaperService
{
    private readonly AppDbContext appDbContext = appDbContext;
    private readonly IMapper mapper = _mapper;
    private readonly HybridCache hybridCache = hybridCache;
    private readonly IVisitorService visitorService = visitorService;
    private readonly ProfileImages profileOptions = profileOptions.Value;
    private readonly ArticleImages imagesOptions = imagesOptions.Value;
    private readonly string prefixNewspaperCache = nameof(prefixNewspaperCache);
    //N1
    public async Task<Result<FullArticleFullResponse>> PostFullArticle
        (string newsapperID, AddFullArticleRequest addFullArticleRequest, CancellationToken cancellationToken = default)
    {
        var article = await PostArticle(newsapperID, addFullArticleRequest.ArticleRequest, cancellationToken);
        if (article.IsFailure)
            return Result.Failure<FullArticleFullResponse>(article.Error);

        var newspaper = await appDbContext.NewsPapers.FindAsync(newsapperID);

        var simpleNotificationMessage = new SimpleNotificationRequest(
            article.Value.Id,
            ConstantStrings.NewspaperPostNewArticle(newspaper!.Name).Title,
            ConstantStrings.NewspaperPostNewArticle(newspaper!.Name).Message);

        Result<List<TagResponse>>? tags = null;
        if (addFullArticleRequest.Tags.Any())
        {
            tags = await SetTagForArticle(newsapperID, article.Value.Id, addFullArticleRequest.Tags, cancellationToken);
            if (tags.IsFailure)
                return Result.Failure<FullArticleFullResponse>(tags.Error);
        }

        Result<List<ImageResponse>>? images = null;
        if (addFullArticleRequest.Images.Any())
        {
            images = await SetImagesForArticle(newsapperID, article.Value.Id, addFullArticleRequest.Images, cancellationToken);
            if (images.IsFailure)
                return Result.Failure<FullArticleFullResponse>(images.Error);
        }

        var fullArticle = new FullArticleFullResponse(
            article.Value.Id, article.Value.Title, article.Value.Description,
            addFullArticleRequest.ArticleRequest.Content, article.Value.Newspaper, article.Value.Category,
            article.Value.IsPosted, article.Value.PostDate, article.Value.Likes, article.Value.Views, article.Value.Comments,
            tags is not null ? tags.Value : new List<TagResponse>(),
            images is not null ? images.Value : new List<ImageResponse>()
            );

        var notificationUsers = await NotificationFollowersForNewArticle(newsapperID, simpleNotificationMessage, cancellationToken);

        await hybridCache.RemoveAsync($"{prefixNewspaperCache}/{nameof(CategoryUsed)}/{newsapperID}", cancellationToken);
        await hybridCache.RemoveAsync($"{prefixNewspaperCache}/{nameof(CountOfPublishedArticles)}/{newsapperID}");
        await hybridCache.RemoveAsync($"{prefixNewspaperCache}/{nameof(UsedTagsForNewspaper)}/{newsapperID}");
        await hybridCache.RemoveAsync($"prefixCacheVisitor/ArticleByNewspaper/{newsapperID}/{false}");
        await hybridCache.RemoveByTagAsync("get_all_notifications_for_member");
        await hybridCache.RemoveByTagAsync("count_of_notifications_for_member");
        await hybridCache.RemoveByTagAsync("show_articles");
        await hybridCache.RemoveByTagAsync("articles_of_newspaper");
        return Result.Success(fullArticle);
    }

    //N2
    public async Task<Result<List<ImageResponse>>> SetImagesForArticle
        (string newspaperID, string articleID, List<FullImageRequest> fullImageRequests, CancellationToken cancellationToken = default)
    {
        if (fullImageRequests.Count >= imagesOptions.MaximumImageCount)
            return Result.Failure<List<ImageResponse>>(ImageErrors.TooManyImages(imagesOptions.MaximumImageCount));

        if (await appDbContext.NewsPapers.AnyAsync(x => x.UserID == newspaperID && !x.IsActive, cancellationToken))
            return Result.Failure<List<ImageResponse>>(NewspaperErrors.NotActicatedNewspaper);

        if (!(await appDbContext.Articles.AnyAsync(x => x.Id == articleID && x.NewsPaperID == newspaperID, cancellationToken)))
            return Result.Failure<List<ImageResponse>>(ArticleErrors.NotFoundArticle);

        if (fullImageRequests.Count != fullImageRequests.DistinctBy(x => x.Placement).Count())
            return Result.Failure<List<ImageResponse>>(ImageErrors.DuplicatedPlacement);

        //DeleteImage
        var removeImages = await appDbContext.Images.Where(x => x.ArticleID == articleID).ToListAsync(cancellationToken);
        removeImages.ForEach((image) =>
        {
            var fullPath = $"{imagesOptions.Path}\\{image.Name}";
            var deleteImage = ImageHelper.Delete(fullPath);
        });

        //RemoveImage
        appDbContext.Images.RemoveRange(removeImages);

        //SaveAndAddImages
        var imageResponses = new List<ImageResponse>();
        var images = new List<Image>();

        var imageTasks = fullImageRequests.Select(async item =>
        {
            var saveResult = await ImageHelper.Save(item.File, imagesOptions);
            if (!saveResult.IsSuccess)
                return null;
            var image = new Image
            {
                Placement = item.Placement,
                ArticleID = articleID,
                Name = saveResult.Value.imageName
            };
            var imageResponse = new ImageResponse(image.Id, saveResult.Value.imageName, item.Placement, articleID);
            images.Add(image);
            imageResponses.Add(imageResponse);
            return new
            {
                image,
                imageResponse
            };
        }).ToList();
        var savedImages = (await Task.WhenAll(imageTasks)).Where(x => x != null).ToList();

        var temp = images.ToArray();
        await appDbContext.Images.AddRangeAsync(temp, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);

        await hybridCache.RemoveByTagAsync("read_article", cancellationToken);
        await hybridCache.RemoveAsync($"{prefixNewspaperCache}/{nameof(CategoryUsed)}/{newspaperID}", cancellationToken);
        await hybridCache.RemoveAsync($"prefixCacheVisitor/ArticleByNewspaper/{newspaperID}/{false}");
        await hybridCache.RemoveByTagAsync("show_articles");
        await hybridCache.RemoveByTagAsync("articles_of_newspaper");
        await hybridCache.RemoveByTagAsync($"{articleID}", cancellationToken);
        await hybridCache.RemoveByTagAsync($"get_article_image_{articleID}", cancellationToken);

        return Result.Success(imageResponses);
    }

    //N3
    public async Task<Result<List<TagResponse>>> SetTagForArticle
        (string newspaperID, string articleID, List<TagRequest> tagRequests, CancellationToken cancellationToken = default)
    {
        if (tagRequests.Count > 50)
            return Result.Failure<List<TagResponse>>(TagErrors.TooManyTag);

        if (await appDbContext.NewsPapers.AnyAsync(x => x.UserID == newspaperID && !x.IsActive, cancellationToken))
            return Result.Failure<List<TagResponse>>(NewspaperErrors.NotActicatedNewspaper);

        tagRequests = tagRequests.Select(x => new TagRequest(x.TagName.ToLower())).ToList();

        var articleExists = await appDbContext.Articles
            .AnyAsync(x => x.Id == articleID && x.NewsPaperID == newspaperID, cancellationToken);
        if (!articleExists)
            return Result.Failure<List<TagResponse>>(ArticleErrors.NotFoundArticle);

        if (tagRequests.Count != tagRequests.DistinctBy(x => x.TagName.Trim()).Count())
            return Result.Failure<List<TagResponse>>(TagErrors.DuplicatedTag);

        if (!tagRequests.Any())
        {
            await appDbContext.ArticlesTags.Where(x => x.ArticleID == articleID).ExecuteDeleteAsync(cancellationToken);
            return Result.Success(new List<TagResponse>());
        }

        //RequestTags
        var requestedTagNames = tagRequests.Select(x => x.TagName.Trim()).Distinct().ToList();

        //CurrentExistTags
        var existingTags = await appDbContext.Tags
            .Where(t => requestedTagNames.Contains(t.TagName))
            .ToListAsync(cancellationToken);

        //NewTags
        var newTagNames = requestedTagNames
            .Except(existingTags.Select(t => t.TagName))
            .ToList();

        //AddNewTags
        if (newTagNames.Any())
        {
            var newTags = newTagNames.Select(name => new Tag { TagName = name });
            await appDbContext.Tags.AddRangeAsync(newTags, cancellationToken);
            await appDbContext.SaveChangesAsync(cancellationToken); // Save to get IDs
        }

        //DeleteAllTagsForArticle
        await appDbContext.ArticlesTags
            .Where(x => x.ArticleID == articleID)
            .ExecuteDeleteAsync(cancellationToken);

        //GetAllTags
        var allTags = await appDbContext.Tags
            .Where(t => requestedTagNames.Contains(t.TagName))
            .ToListAsync(cancellationToken);

        //CreateListArticleTags
        var articleTags = allTags
            .Select(tag => new ArticlesTags { ArticleID = articleID, TagID = tag.Id })
            .ToList();

        //AddArticleTag
        await appDbContext.ArticlesTags.AddRangeAsync(articleTags, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);

        var response = allTags.ToTagsResponses(mapper).ToList();

        await hybridCache.RemoveAsync($"{prefixNewspaperCache}/{nameof(UsedTagsForNewspaper)}/{newspaperID}", cancellationToken);
        await hybridCache.RemoveAsync($"{prefixNewspaperCache}/{nameof(CategoryUsed)}/{newspaperID}", cancellationToken);
        await hybridCache.RemoveAsync($"prefixCacheVisitor/ArticleByNewspaper/{newspaperID}/{false}");
        await hybridCache.RemoveByTagAsync("show_articles");
        await hybridCache.RemoveByTagAsync($"read_{articleID}");
        await hybridCache.RemoveByTagAsync("articles_of_newspaper");
        await hybridCache.RemoveByTagAsync($"{articleID}", cancellationToken);

        return Result.Success(response);
    }

    //N4
    public async Task<Result> UpdateArticle
        (string newspapreID, string articleID, ArticleRequest articleRequest, CancellationToken cancellationToken = default)
    {
        if (!(await appDbContext.Categories.AnyAsync(x => x.Id == articleRequest.CategoryID, cancellationToken)))
            return Result.Failure(CategoryErrors.NotFoundCategory);

        if (await appDbContext.NewsPapers.AnyAsync(x => x.UserID == newspapreID && !x.IsActive))
            return Result.Failure(NewspaperErrors.NotActicatedNewspaper);

        if (!(await appDbContext.Articles.AnyAsync(x => x.Id == articleID && x.NewsPaperID == newspapreID, cancellationToken)))
            return Result.Failure(ArticleErrors.NotFoundArticle);

        await appDbContext.Articles.Where(x => x.Id == articleID)
            .ExecuteUpdateAsync(setters =>
                setters
                .SetProperty(x => x.Title, articleRequest.Title)
                .SetProperty(x => x.Descrpition, articleRequest.Description)
                .SetProperty(x => x.CategoryID, articleRequest.CategoryID)
                .SetProperty(x => x.Content, articleRequest.Content)
                .SetProperty(x => x.LastUpdate, DateTime.UtcNow)
            );

        await hybridCache.RemoveAsync($"prefixCacheVisitor/ArticleByNewspaper/{newspapreID}/{false}", cancellationToken);
        await hybridCache.RemoveAsync($"prefixCacheVisitor/ArticleByNewspaper/{newspapreID}/{true}", cancellationToken);
        await hybridCache.RemoveByTagAsync("count_likes_by_categories", cancellationToken);
        await hybridCache.RemoveByTagAsync("count_views_by_categories", cancellationToken);
        await hybridCache.RemoveByTagAsync("show_articles", cancellationToken);
        await hybridCache.RemoveByTagAsync($"read_{articleID}", cancellationToken);
        await hybridCache.RemoveByTagAsync("used_categories", cancellationToken);
        
        return Result.Success();
    }

    //N5
    public async Task<Result<bool>> TogglePostStatus
        (string? newspaperID, string articleID, CancellationToken cancellationToken = default)
    {
        if (newspaperID is null)
            if (await appDbContext.Articles.FindAsync(articleID, cancellationToken) is not { } artcl)
                return Result.Failure<bool>(ArticleErrors.NotFoundArticle);
            else
                await appDbContext.Articles.Where(x => x.Id == artcl.Id).ExecuteUpdateAsync(
                    setters =>
                        setters.SetProperty(x => x.IsPosted, !artcl.IsPosted),
                    cancellationToken
                );

        if (await appDbContext.NewsPapers.AnyAsync(x => x.UserID == newspaperID && !x.IsActive))
                return Result.Failure<bool>(NewspaperErrors.NotActicatedNewspaper);

        if (await appDbContext.Articles.FindAsync(articleID, cancellationToken) is not { } article)
            return Result.Failure<bool>(ArticleErrors.NotFoundArticle);

        if(article.NewsPaperID != newspaperID)
            return Result.Failure<bool>(ArticleErrors.NotFoundArticle);

        await appDbContext.Articles.Where(x => x.Id == articleID)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(x => x.IsPosted, !article.IsPosted)
            );

        await hybridCache.RemoveByTagAsync($"articles_of_newspaper_{newspaperID}", cancellationToken);
        await hybridCache.RemoveByTagAsync($"comments_for_article_{articleID}", cancellationToken);
        await hybridCache.RemoveByTagAsync("member_comments", cancellationToken);
        await hybridCache.RemoveByTagAsync("liked_articles", cancellationToken);
        await hybridCache.RemoveByTagAsync("saved_articles", cancellationToken);
        await hybridCache.RemoveByTagAsync($"show_member_likes_{articleID}", cancellationToken);
        await hybridCache.RemoveByTagAsync($"get_article_image_{articleID}", cancellationToken);
        await hybridCache.RemoveByTagAsync($"read_{articleID}", cancellationToken);
        await hybridCache.RemoveByTagAsync("show_article", cancellationToken);
        await hybridCache.RemoveAsync($"{prefixNewspaperCache}/{nameof(CountOfPublishedArticles)}/{newspaperID}", cancellationToken);

        return Result.Success(!article.IsPosted);
    }
    
    //N6
    public async Task<Result<int>> CalculateLikesForCategory
        (string newspaperID, string categoryID, CancellationToken cancellationToken = default)
    {

        if (!(await appDbContext.Categories.AnyAsync(x => x.Id == categoryID, cancellationToken)))
            return Result.Failure<int>(CategoryErrors.NotFoundCategory);

        var countKey = $"{prefixNewspaperCache}/{nameof(CalculateLikesForCategory)}/{newspaperID}/{categoryID}";
        var cache = await hybridCache.GetOrCreateAsync(
            countKey,
            async entry =>
                await (from article in appDbContext.Articles.Select(x => new { x.Id, x.AllLikes, x.NewsPaperID, x.CategoryID })
                       where article.CategoryID == categoryID && article.NewsPaperID == newspaperID
                       select article.AllLikes)
                    .SumAsync(cancellationToken),
            cancellationToken: cancellationToken,
            tags: ["count_likes_by_categories"]
        );

        return Result.Success(cache);
    }

    //N7
    public async Task<Result<int>> CalculateViewsForCategory
        (string newspaperID, string categoryID, CancellationToken cancellationToken = default)
    {
        if (!(await appDbContext.Categories.AnyAsync(x => x.Id == categoryID, cancellationToken)))
            return Result.Failure<int>(CategoryErrors.NotFoundCategory);

        var countKey = $"{prefixNewspaperCache}/{nameof(CalculateViewsForCategory)}/{newspaperID}/{categoryID}";
        var cache = await hybridCache.GetOrCreateAsync(
            countKey,
            async entry =>
                await (from article in appDbContext.Articles.Select(x => new { x.Id, x.Views, x.NewsPaperID, x.CategoryID })
                        where article.CategoryID == categoryID && article.NewsPaperID == newspaperID
                        select article.Views)
                        .SumAsync(cancellationToken),
            cancellationToken: cancellationToken,
            tags: ["count_views_by_categories", $"count_views_by_categories_{newspaperID}"]
        );
        return Result.Success(cache);
    }

    //N8
    public async Task<Result<PaginatedList<CategoryWithCountOfUseResponse>>> CategoryUsed
        (string newspaperID, ResultFilter resultFilter, CancellationToken cancellationToken = default)
    {
        var countKey = $"{prefixNewspaperCache}/{nameof(CategoryUsed)}/{newspaperID}";
        var cache = await hybridCache.GetOrCreateAsync(
            countKey,
            async entry =>
                await appDbContext.Articles
                    .AsNoTracking()
                    .Where(x => x.NewsPaperID == newspaperID)
                    .GroupBy(x => x.Category.CategoryName)
                    .Select(g => new { CategoryName = g.Key, Count = g.Count() })
                    .OrderBy(x => x.CategoryName)
                    .Select(x => new CategoryWithCountOfUseResponse(x.CategoryName, x.Count))
                    .ToListAsync(cancellationToken),
            cancellationToken: cancellationToken,
            tags: ["used_categories"]
        );

        var result = PaginatedList<CategoryWithCountOfUseResponse>.Create(cache, resultFilter.PageNumber, resultFilter.PageSize);
        return Result.Success(result);
    }

    //N9
    public async Task<Result<int>> CountOfLikes
        (string newspaperID, CancellationToken cancellationToken = default)
    {
        var countKey = $"{prefixNewspaperCache}/{nameof(CountOfLikes)}/{newspaperID}";
        var cache = await hybridCache.GetOrCreateAsync(
            countKey,
            async entry =>
                await appDbContext.Articles.AsNoTracking()
                .Where(x => x.NewsPaperID == newspaperID)
                .Select(x => x.AllLikes).SumAsync(cancellationToken),
            cancellationToken: cancellationToken,
            tags: [$"count_of_likes_{newspaperID}"]
        );

        return Result.Success(cache);
    }

    //N10
    public async Task<Result<int>> CountOfViews
        (string newspaperID, CancellationToken cancellationToken = default)
    {
        var countKey = $"{prefixNewspaperCache}/{nameof(CountOfViews)}/{newspaperID}";
        var cache = await hybridCache.GetOrCreateAsync(
            countKey,
            async entry =>
                await appDbContext.Articles.AsNoTracking()
                .Where(x => x.NewsPaperID == newspaperID)
                .Select(x => x.Views).SumAsync(cancellationToken),
            cancellationToken: cancellationToken,
            tags: [$"count_of_views_{ newspaperID}"]
        );

        return Result.Success(cache);
    }

    //N11
    public async Task<Result<int>> CountOfPublishedArticles
        (string newspaperID, CancellationToken cancellationToken = default)
    {
        var countKey = $"{prefixNewspaperCache}/{nameof(CountOfPublishedArticles)}/{newspaperID}";
        var cache = await hybridCache.GetOrCreateAsync(
            countKey,
            async entry =>
                await appDbContext.Articles.AsNoTracking()
                .Where(x => x.NewsPaperID == newspaperID && x.IsPosted)
                .CountAsync(cancellationToken),
            cancellationToken: cancellationToken
        );

        return Result.Success(cache);
    }

    //N12
    public async Task<Result<PaginatedList<TagResponse>>> UsedTagsForNewspaper
        (string newspaperID, ResultFilter resultFilter, CancellationToken cancellationToken = default)
    {
        var tagKey = $"{prefixNewspaperCache}/{nameof(UsedTagsForNewspaper)}/{newspaperID}";
        var cache = await hybridCache.GetOrCreateAsync(
            tagKey,
            async entry =>
            {
                var temp = await appDbContext.ArticlesTags.AsNoTracking()
                    .Join(appDbContext.Articles.Where(x => x.NewsPaperID == newspaperID).Select(x => x.Id),
                        at => at.ArticleID,
                        a => a,
                        (at, a) => new { TagID = at.TagID, ArticleID = a }
                    ).Join(appDbContext.Tags.AsNoTracking(),
                        at => at.TagID,
                        t => t.Id,
                        (at, t) => t
                    ).ToListAsync(cancellationToken);

                var data = temp
                    .Select(x => x.ToTagResponse(mapper))
                    .Distinct()
                    .OrderBy(x => x.TagName)
                    .ToList();
                return data;
            },
            cancellationToken: cancellationToken,
            tags: ["used_tags_for_newspaper"]
        );

        var result = PaginatedList<TagResponse>.Create(cache, resultFilter.PageNumber, resultFilter.PageSize);
        return Result.Success(result);
    }

    //N13
    public async Task<Result> UpdateNewspaper
        (string newspaperID, NewsPaperRequest newsPaperRequest, CancellationToken cancellationToken = default)
    {

        if (await appDbContext.NewsPapers.FindAsync(newspaperID, cancellationToken) is not { } newspaper)
            return Result.Failure(NewspaperErrors.NotFoundNewspaper);

        if (!newspaper.IsActive)
            return Result.Failure(NewspaperErrors.NotActicatedNewspaper);

        if ((await appDbContext.NewsPapers.AnyAsync(x => x.Name == newsPaperRequest.Name &&
            x.UserID != newspaperID, cancellationToken)))
            return Result.Failure(NewspaperErrors.NewspaperNameDuplicated);

        var result = await appDbContext.NewsPapers.Where(x => x.UserID == newspaperID)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.Name, newsPaperRequest.Name)
            );

        return Result.Success();
    }

    //N14
    public async Task<Result<ProfileImageResponse>> ChangeProfileImage
        (string newspaperID, IFormFile formFile, CancellationToken cancellationToken = default)
    {
        if (formFile is null)
            return Result.Failure<ProfileImageResponse>(ImageErrors.NullImage);

        if (await appDbContext.NewsPapers.AnyAsync(x => x.UserID == newspaperID && !x.IsActive, cancellationToken))
            return Result.Failure<ProfileImageResponse>(NewspaperErrors.NotActicatedNewspaper);

        var imageCheck = await appDbContext.ProfileImages.FirstOrDefaultAsync(x => x.userID == newspaperID);
        if (imageCheck is not null)
        {
            await appDbContext.ProfileImages.Where(x => x.userID == newspaperID)
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
            userID = newspaperID,
        };
        var addedImage = await appDbContext.ProfileImages.AddAsync(image, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(addedImage.Entity.ToProfileImageResponse(mapper)!);
    }

    //N15
    public async Task<Result> RemoveProfileImage
        (string newspaperID, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.NewsPapers.AnyAsync(x => x.UserID == newspaperID && !x.IsActive, cancellationToken))
            return Result.Failure(NewspaperErrors.NotActicatedNewspaper);

        var image = await appDbContext.ProfileImages.SingleOrDefaultAsync(x => x.userID == newspaperID, cancellationToken);
        if (image is null)
            return Result.Failure(ImageErrors.NotFoundImage);

        var deleteImage = ImageHelper.Delete($"{profileOptions.Path}\\{image.Name}");
        if (deleteImage.IsFailure)
            return Result.Failure(deleteImage.Error);
        var result = await appDbContext.ProfileImages.Where(x => x.userID == newspaperID)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<Result<ArticleBreifResponse>> PostArticle
        (string newspaperID, ArticleRequest articleRequest, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.NewsPapers.AnyAsync(x => x.UserID == newspaperID && !x.IsActive))
            return Result.Failure<ArticleBreifResponse>(NewspaperErrors.NotActicatedNewspaper);

        if (!(await appDbContext.Categories.AnyAsync(x => x.Id == articleRequest.CategoryID, cancellationToken)))
            return Result.Failure<ArticleBreifResponse>(CategoryErrors.NotFoundCategory);

        var article = new Article
        {
            Title = articleRequest.Title,
            Descrpition = articleRequest.Description,
            Content = articleRequest.Content,
            CategoryID = articleRequest.CategoryID,
            NewsPaperID = newspaperID,
        };
        await appDbContext.Articles.AddAsync(article, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);

        var articleBreifResponse = new ArticleBreifResponse
            (article.Id, article.Title, article.Descrpition,
            (await appDbContext.NewsPapers.SingleAsync(x => x.UserID == newspaperID, cancellationToken)).ToNewspaperBreifResponse(mapper),
            (await appDbContext.Categories.SingleAsync(x => x.Id == article.CategoryID, cancellationToken)).ToCategoryResponse(mapper),
            article.IsPosted, article.PostDate, article.AllLikes, article.Views, article.AllComments,
            new List<TagResponse>(),
            default(ImageResponse)
            );

        return Result.Success(articleBreifResponse);
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////Visistor Services//////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////

    //N16
    public async Task<Result<PaginatedList<ArticleBreifResponse>>> MyArticles
        (string newspaperID, ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await visitorService.ArticleByNewspaper(newspaperID, false, resultFilter, cancellationToken);

    //N17
    public async Task<Result<PaginatedList<MemberUserResponse>>> ShowMembersLikes
        (string newspaperID, string articleID, ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await visitorService.ShowMembersLikes(newspaperID, articleID, false, resultFilter, cancellationToken);

    //N18
    public async Task<Result<MemberUserResponse>> MemberProfile
        (string memberID, CancellationToken cancellationToken = default)
        => await visitorService.MemberProfile(memberID, true, cancellationToken);

    //N19
    public async Task<Result<NewspaperUserResponse>> MyProfile
        (string newspaperID, CancellationToken cancellationToken = default)
        => await visitorService.NewspaperProfile(newspaperID, false, cancellationToken);

    //N20
    public async Task<Result<PaginatedList<CommentViewResponse>>> CommentsForArticle
        (string newspaperID, string articleID, ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await appDbContext.Articles.AnyAsync(x => x.Id == articleID && x.NewsPaperID == newspaperID, cancellationToken)
        ? await visitorService.CommentsForArticle(articleID, false, resultFilter, cancellationToken)
        : Result.Failure<PaginatedList<CommentViewResponse>>(ArticleErrors.NotFoundArticle);

    //N21
    public async Task<Result<FullArticleFullResponse>> ReadArticle
        (string newspaperID, string articleID, CancellationToken cancellationToken = default)
        => await appDbContext.Articles.AnyAsync(x => x.Id == articleID && x.NewsPaperID == newspaperID)
        ? await visitorService.ReadArticle(newspaperID, articleID, false, cancellationToken)
        : Result.Failure<FullArticleFullResponse>(ArticleErrors.NotFoundArticle);

    //N22
    public async Task<Result<PaginatedList<CategoryResponse>>> GetAllCategories
        (ResultFilter resultFilter, CancellationToken cancellationToken = default)
        => await visitorService.GetAllCategories(resultFilter, cancellationToken);

    //N23
    public async Task<Result<(string imagePath, string contentType)>> GetMyProfileImage
        (string newspaperID, CancellationToken cancellationToken = default)
        => await visitorService.GetProfileImage(newspaperID, UserTypes.NewsPaper, false, cancellationToken);

    //N24
    public async Task<Result<(string imagePath, string contentType)>> GetMemberProfileImage
        (string memberID, CancellationToken cancellationToken)
        => await visitorService.GetProfileImage(memberID, UserTypes.Member, true, cancellationToken);

    //N25
    public async Task<Result<(string imagePath, string contentType)>> GetArticleImage
        (string newspaperID, string imageName, CancellationToken cancellationToken)
    {
        var article = (await appDbContext.Images.AsNoTracking()
            .Include(x => x.Article)
            .SingleOrDefaultAsync(x => x.Name == imageName))?.Article;

        if (article is null)
            return Result.Failure<(string, string)>(ImageErrors.NotFoundImage);

        if (article.NewsPaperID != newspaperID)
            return Result.Failure<(string, string)>(ArticleErrors.NotFoundArticle);

        return await visitorService.GetArticleImage(imageName, false, cancellationToken);
    }

    //N26
    public async Task<Result> ReportForMember
        (string newspaperID, ReportForMemberRequest reportForMemberRequest, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.NewsPapers.AnyAsync(x => x.UserID == newspaperID && !x.IsActive))
            return Result.Failure(NewspaperErrors.NotActicatedNewspaper);
        
        if (!(await appDbContext.Members.AnyAsync(x => x.UserID == reportForMemberRequest.MemberID && x.IsActive)))
            return Result.Failure(MemberErrors.NotFoundMember);

        var notification = new Notification
        {
            NotificationTypes = NotificationTypes.ReportNotification.ToString(),
            Title = reportForMemberRequest.Title,
            Message = reportForMemberRequest.Message,
            EntityID = reportForMemberRequest.MemberID,
            EntityType = EntitiesTypes.User.ToString(),
            ReceiveUserID = DefaultUsers.AdminID,
            SenderUserID = newspaperID
        };

        await appDbContext.Notifications.AddAsync(notification, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);
        
        return  Result.Success();
    }

    //N27
    public async Task<Result> ReportForAdmin
        (string newspaperID, AskForHelpServiceRequest askForHelpService, CancellationToken cancellationToken = default)
    {
        if (await appDbContext.NewsPapers.AnyAsync(x => x.UserID == newspaperID && !x.IsActive))
            return Result.Failure(NewspaperErrors.NotActicatedNewspaper);

        var notification = new Notification
        {
            NotificationTypes = NotificationTypes.AskServiceNotification.ToString(),
            Title = askForHelpService.Title,
            Message = askForHelpService.Message,
            ReceiveUserID = DefaultUsers.AdminID,
            SenderUserID = newspaperID
        };

        await appDbContext.Notifications.AddAsync(notification, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    //N28
    public async Task<Result<PaginatedList<NotificationResponse>>> GetAllNotification
        (string newspaperID, bool isReaded, ResultFilter resultFilter, CancellationToken cancellationToken = default)
    {

        var notificationskey = $"{prefixNewspaperCache}/{nameof(GetAllNotification)}/{newspaperID}/{isReaded}";
        var cache = await hybridCache.GetOrCreateAsync(
            notificationskey,
            async entry =>
            {
                var temp = appDbContext.Notifications.AsNoTracking()
                    .Where(x => x.ReceiveUserID == newspaperID && isReaded ? !x.IsReaded : true)
                    .OrderByDescending(x => x.CreatedAt).ThenBy(x => x.IsReaded);
                var data = await temp.Select(x =>
                        new NotificationResponse(x.Id, x.Title, x.Message, x.NotificationTypes, x.EntityID, x.EntityType, x.CreatedAt, x.IsReaded)
                    ).ToListAsync(cancellationToken);
                return data;
            },
            cancellationToken: cancellationToken,
            tags: ["get_all_notifications_for_newspaper", $"get_all_notifications_for_newspaper_{newspaperID}"]
        );
        var result = PaginatedList<NotificationResponse>.Create(cache, resultFilter.PageNumber, resultFilter.PageSize);
        return Result.Success(result);
    }

    //N29
    public async Task<Result> ReadAllNotifications
        (string newspaperID, CancellationToken cancellationToken = default)
    {
        var temp = await appDbContext.Notifications
            .Where(x => x.ReceiveUserID == newspaperID && !x.IsReaded)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.IsReaded, true)
            );
        await hybridCache.RemoveByTagAsync($"get_all_notifications_for_newspaper_{newspaperID}");
        await hybridCache.RemoveByTagAsync("count_of_notifications");
        return Result.Success();
    }

    //N30
    public async Task<Result> ReadNotification
        (string newspaperID, string notificationID, CancellationToken cancellationToken = default)
    {
        if (!(await appDbContext.Notifications.AnyAsync(x => x.Id == notificationID && x.ReceiveUserID == newspaperID, cancellationToken)))
            return Result.Failure(NotificationErrors.NotificationNotFound);

        if (await appDbContext.Notifications.AnyAsync(x => x.Id == notificationID && x.IsReaded, cancellationToken))
            return Result.Failure(NotificationErrors.NotificationAlreadyReaded);

        var temp = await appDbContext.Notifications
            .Where(x => x.Id == notificationID && !x.IsReaded)
            .ExecuteUpdateAsync(setters =>
                setters
                    .SetProperty(x => x.IsReaded, true)
            );
        await hybridCache.RemoveByTagAsync($"all_of_notifications");
        await hybridCache.RemoveByTagAsync($"get_all_notifications_for_newspaper_{newspaperID}");
        await hybridCache.RemoveByTagAsync("count_of_notifications");
        return Result.Success();
    }

    //N31
    public async Task<Result<int>> CountOfNotifications
        (string newspaperID, bool isReaded, CancellationToken cancellationToken = default)
    {
        var countKey = $"{prefixNewspaperCache}/{nameof(CountOfNotifications)}/{newspaperID}/{isReaded}";
        var cache = await hybridCache.GetOrCreateAsync(
            countKey,
            async entry =>
                await appDbContext.Notifications.AsNoTracking()
                    .CountAsync(x => x.ReceiveUserID == newspaperID && isReaded ? !x.IsReaded : true),
            cancellationToken: cancellationToken,
            tags: ["count_of_notifications_for_newspaper"]
        );
        return Result.Success(cache);
    }

    private async Task<Result> NotificationFollowersForNewArticle
        (string newspaperID, SimpleNotificationRequest simpleNotificationRequest, CancellationToken cancellationToken = default)
    {
        var followers = await appDbContext.Followers.AsNoTracking()
            .Where(x => x.NewsPaperID == newspaperID)
            .Join(appDbContext.Members.AsNoTracking(),
                f => f.MemberID,
                m => m.UserID,
                (f, m) => new {MemberID = m.UserID, MemberStatus = m.IsActive}
            ).Where(x => x.MemberStatus).Select(x => x.MemberID)
            .ToListAsync(cancellationToken);

        var notifications = followers.Select(x => new Notification
        {
            NotificationTypes = NotificationTypes.PostNotification.ToString(),
            Title = simpleNotificationRequest.Title,
            Message = simpleNotificationRequest.Message,
            ReceiveUserID = x,
            SenderUserID = newspaperID,
            EntityID = simpleNotificationRequest.EntityID,
            EntityType = EntitiesTypes.Article.ToString(),
        });
        await appDbContext.Notifications.AddRangeAsync(notifications, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();    
    }
}
