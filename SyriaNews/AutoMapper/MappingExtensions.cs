
namespace SyriaNews.AutoMapper;

public static class MappingExtensions
{
    //role
    public static RoleResponse ToRoleResponse(this ApplicationRole applicationRole, IMapper mapper)
        => mapper.Map<RoleResponse>(applicationRole);

    //article
    public static async Task<ArticleBreifResponse> ToArticleBreifResponse
        (this Article articles, AppDbContext appDbContext, IMapper mapper, CancellationToken cancellationToken = default)
    {
        var newspaper = await appDbContext.NewsPapers.AsNoTracking().SingleAsync(x => x.UserID == articles.NewsPaperID, cancellationToken);
        var category = await appDbContext.Categories.AsNoTracking().SingleAsync(x => x.Id == articles.CategoryID, cancellationToken);
        var tagsList = await (from tag in appDbContext.Tags.AsNoTracking()
                        join articleTag in appDbContext.ArticlesTags.AsNoTracking()
                        on tag.Id equals articleTag.TagID
                        where articleTag.ArticleID == articles.Id
                        select tag).ToListAsync(cancellationToken);

        var firstArticleImage = await appDbContext.Images.AsNoTracking().FirstOrDefaultAsync(x => x.ArticleID == articles.Id, cancellationToken);
        return new ArticleBreifResponse(
            articles.Id, articles.Title, articles.Descrpition, newspaper.ToNewspaperBreifResponse(mapper),
            category.ToCategoryResponse(mapper), articles.IsPosted, articles.PostDate, articles.AllLikes,
            articles.Views, articles.AllComments, tagsList.ToTagsResponses(mapper).ToList(),
            firstArticleImage?.ToArticleImageResponse(mapper)
        );

    }
    public static async Task<IEnumerable<ArticleBreifResponse>> ToArticlesBreifResponses
        (this IEnumerable<Article> articles, AppDbContext appDbContext, IMapper mapper, CancellationToken cancellationToken = default)
    {

        var temp = articles.Select(x => x.ToArticleBreifResponse(appDbContext, mapper, cancellationToken));
        var result = await Task.WhenAll(temp);
        return result;
    }
    public static async Task<FullArticleFullResponse> ToFullArticleFullResponse
        (this Article articles, AppDbContext appDbContext, IMapper mapper, CancellationToken cancellationToken = default)
    {
        return new FullArticleFullResponse(
            articles.Id, articles.Title, articles.Descrpition, articles.Content,
            (await appDbContext.NewsPapers.AsNoTracking().SingleAsync(x => x.UserID == articles.NewsPaperID, cancellationToken)).ToNewspaperBreifResponse(mapper),
            (await appDbContext.Categories.AsNoTracking().SingleAsync(x => x.Id == articles.CategoryID, cancellationToken)).ToCategoryResponse(mapper),
            articles.IsPosted, articles.PostDate, articles.AllLikes, articles.Views, articles.AllComments,
            (await appDbContext.ArticlesTags.AsNoTracking().Where(x => x.ArticleID == articles.Id)
            .Join(appDbContext.Tags.AsNoTracking(), at => at.TagID, t => t.Id, (at, t) => t.ToTagResponse(mapper)).ToListAsync(cancellationToken)),
            (await appDbContext.Images.AsNoTracking().Where(x => x.ArticleID == articles.Id).ToListAsync(cancellationToken)).ToImagesResponse(mapper).ToList()
        );
    }

    //Newspaper
    public static NewspaperBreifResponse ToNewspaperBreifResponse(this NewsPaper newsPaper, IMapper mapper)
        => new NewspaperBreifResponse(newsPaper.UserID, newsPaper.Name, newsPaper.followers, newsPaper.IsActive);
        
    //Member
    public static MemberBreifResponse ToMemberBreifResponse(this Member member, IMapper mapper)
        => mapper.Map<MemberBreifResponse>(member);

    //Category
    public static CategoryResponse ToCategoryResponse(this Category category, IMapper mapper)
        => mapper.Map<CategoryResponse>(category);
    public static Category ToCategory(this CategoryRequest categoryRequest, IMapper mapper)
        => mapper.Map<Category>(categoryRequest);
    public static CategoryRequest ToCategoryRequest(this Category category, IMapper mapper)
        => mapper.Map<CategoryRequest>(category);
    public static IEnumerable<Category> ToCategories(this IEnumerable<CategoryRequest> categoryRequests, IMapper mapper)
        => categoryRequests.Select(x => mapper.Map<Category>(x));
    public static IEnumerable<CategoryRequest> ToCategoriesRequests(this IEnumerable<Category> categories, IMapper mapper)
        => categories.Select(x => mapper.Map<CategoryRequest>(x));
    public static IEnumerable<CategoryResponse> ToCategoriesRespones(this IEnumerable<Category> categories, IMapper mapper)
        => categories.Select(x => mapper.Map<CategoryResponse>(x));

    //Tag
    public static Tag ToTag(this TagRequest tagRequest, IMapper mapper)
        => mapper.Map<Tag>(tagRequest);
    public static Tag ToTag(this TagResponse tagResponse, IMapper mapper)
        => mapper.Map<Tag>(tagResponse);
    public static TagRequest ToTagRequest(this Tag tag, IMapper mapper)
        => mapper.Map<TagRequest>(tag);
    public static TagResponse ToTagResponse(this Tag tag, IMapper mapper)
        => mapper.Map<TagResponse>(tag);
    public static IEnumerable<TagResponse> ToTagsResponses(this IEnumerable<Tag> tags, IMapper mapper)
        => tags.Select(x => mapper.Map<TagResponse>(x));

    //Save
    public static Save ToSave(this SaveRequest saveRequest, IMapper mapper)
        => mapper.Map<Save>(saveRequest);
    public static Save ToSave(this SaveResponse saveResponse, IMapper mapper)
        => mapper.Map<Save>(saveResponse);
    public static SaveRequest ToSaveRequest(this Save save, IMapper mapper)
        => mapper.Map<SaveRequest>(save);
    public static SaveResponse ToSaveResponse(this Save save, IMapper mapper)
        => mapper.Map<SaveResponse>(save);
    public static IEnumerable<SaveResponse> ToSavesResponses(this IEnumerable<Save> saves, IMapper mapper)
        => saves.Select(x => mapper.Map<SaveResponse>(x));

    //Like
    public static Like ToLike(this LikeRequest likeRequest, IMapper mapper)
        => mapper.Map<Like>(likeRequest);
    public static Like ToLike(this LikeResponse likeResponse, IMapper mapper)
        => mapper.Map<Like>(likeResponse);
    public static LikeRequest ToLikeRequest(this Like like, IMapper mapper)
        => mapper.Map<LikeRequest>(like);
    public static LikeResponse ToLikeResponse(this Like like, IMapper mapper)
        => mapper.Map<LikeResponse>(like);
    public static IEnumerable<LikeResponse> ToLikeResponses(this IEnumerable<Like> likes, IMapper mapper)
        => likes.Select(x => mapper.Map<LikeResponse>(x));

    //Follower
    public static Follower ToFollower(this FollowerRequest followerRequest, IMapper mapper)
        => mapper.Map<Follower>(followerRequest);
    public static Follower ToFollower(this FollowerResponse followerResponse, IMapper mapper)
        => mapper.Map<Follower>(followerResponse);
    public static FollowerRequest ToFollowerRequest(this Follower follower, IMapper mapper)
        => mapper.Map<FollowerRequest>(follower);
    public static FollowerResponse ToFollowerResponse(this Follower follower, IMapper mapper)
        => mapper.Map<FollowerResponse>(follower);
    public static IEnumerable<FollowerResponse> ToFollowerResponses(this IEnumerable<Follower> followers, IMapper mapper)
        => followers.Select(x => mapper.Map<FollowerResponse>(x));

    //Comment
    public static Comment ToComment(this CommentRequest commentRequest, IMapper mapper)
        => mapper.Map<Comment>(commentRequest);
    public static Comment ToComment(this CommentResponse commentResponse, IMapper mapper)
        => mapper.Map<Comment>(commentResponse);
    public static CommentRequest ToCommentRequest(this Comment comment, IMapper mapper)
        => mapper.Map<CommentRequest>(comment);
    public static CommentResponse ToCommentResponse(this Comment comment, IMapper mapper)
        => mapper.Map<CommentResponse>(comment);
    public static IEnumerable<CommentResponse> ToCommentResponses(this IEnumerable<Comment> comments, IMapper mapper)
        => comments.Select(x => mapper.Map<CommentResponse>(x));

    //ArticleTag
    public static ArticlesTags ToArticleTag(this ArticleTagRequest articleTagRequest, IMapper mapper)
        => mapper.Map<ArticlesTags>(articleTagRequest);
    public static ArticlesTags ToArticleTag(this ArticleTagResponse articleTagResponse, IMapper mapper)
        => mapper.Map<ArticlesTags>(articleTagResponse);
    public static ArticleTagRequest ToArticlesTagRequest(this ArticlesTags articlesTags, IMapper mapper)
        => mapper.Map<ArticleTagRequest>(articlesTags);
    public static ArticleTagResponse ToArticleTagResponse(this ArticlesTags articlesTags, IMapper mapper)
        => mapper.Map<ArticleTagResponse>(articlesTags);
    public static IEnumerable<ArticleTagResponse> ToArticlesTagsResponses(this IEnumerable<ArticlesTags> articlesTags, IMapper mapper)
        => articlesTags.Select(x => mapper.Map<ArticleTagResponse>(x));

    //ArticleImage
    public static Image ToArticleImage(this ImageRequest imageRequest, IMapper mapper)
        => mapper.Map<Image>(imageRequest);
    public static Image ToArticleImage(this ImageResponse imageResponse, IMapper mapper)
        => mapper.Map<Image>(imageResponse);
    public static ImageResponse ToArticleImageResponse(this Image image, IMapper mapper)
        => mapper.Map<ImageResponse>(image);
    public static IEnumerable<ImageResponse> ToImagesResponse(this IEnumerable<Image> images, IMapper mapper)
        => images.Select(x => mapper.Map<ImageResponse>(x));
    
    //ProfileImage
    public static ProfileImageResponse? ToProfileImageResponse(this ProfileImage? profileImage, IMapper mapper)
        => profileImage is not null ? mapper.Map<ProfileImageResponse>(profileImage) : default;
    public static ProfileImage ToProfileImage(this ProfileImageRequest profileImageRequest, IMapper mapper)
        => mapper.Map<ProfileImage>(profileImageRequest);
    public static ProfileImage ToProfileImage(this ProfileImageResponse profileImageResponse, IMapper mapper)
        => mapper.Map<ProfileImage>(profileImageResponse);
    public static IEnumerable<ProfileImageResponse> ToProfileImagesResponses(this IEnumerable<ProfileImage> profileImages, IMapper mapper)
        => profileImages.Select(x => mapper.Map<ProfileImageResponse>(x));

}
