namespace SyriaNews.Repository.Interfaces;

public interface IMemberService
{

    //M1
    Task<Result<PaginatedList<FullSaveResponse>>> FullSavedArtciels
        (string memberID, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //M2
    Task<Result<PaginatedList<ArticleBreifResponse>>> LikedArticle
        (string memberID, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //M3
    Task<Result<PaginatedList<NewspaperUserResponse>>> FollowedNewspapers
        (string memberID, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //M4
    Task<Result<FullCommentResponse>> CommentOnArticle
        (string memberID, CommentRequest commentRequest, CancellationToken cancellationToken = default);

    //M5
    Task<Result<FullLikeResponse>> LikeArticle
        (string memberID, LikeRequest likeRequest, CancellationToken cancellationToken = default);

    //M6
    Task<Result<FullFollowerResponse>> FollowingNewspaper
        (string memberID, FollowerRequest followerRequest, CancellationToken cancellationToken = default);

    //M7
    Task<Result> UpdateComment
        (string memberID, string commentID, CommentUpdateRequest commentUpdateRequest, CancellationToken cancellationToken = default);

    //M8
    Task<Result> RemoveComment
        (string memberID, string commentID, CancellationToken cancellationToken = default);

    //M9
    Task<Result> RemoveLike
        (string memberID, string articleID, CancellationToken cancellationToken = default);

    //M10
    Task<Result> RemoveFollow
        (string memberID, string newspaperID, CancellationToken cancellationToken = default);

    //M11
    Task<Result<SaveResponse>> SaveArticle
        (string memberID, SaveRequest saveRequest, CancellationToken cancellationToken = default);

    //M12
    Task<Result> UnsaveArticle
        (string memberID, string articleID, CancellationToken cancellationToken = default);

    //M13
    Task<Result> UpdateMemberProfile
        (string memberID, MemberRequest memberRequest, CancellationToken cancellationToken = default);

    //M14
    Task<Result<PaginatedList<FullCommentResponse>>> MemberComments
        (string memberID, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //M15
    Task<Result<ProfileImageResponse>> ChangeProfileImage
        (string memberID, IFormFile formFile, CancellationToken cancellationToken = default);

    //M16
    Task<Result> RemoveProfileImage
        (string memberID, CancellationToken cancellationToken = default);

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////Visistor Services//////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////

    //M17
    Task<Result<PaginatedList<ArticleBreifResponse>>> ShowNewArticlesByPostDate
        (ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //M18
    Task<Result<PaginatedList<ArticleBreifResponse>>> ShowNewArticlesByMostViewed
        (ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //M19
    Task<Result<PaginatedList<ArticleBreifResponse>>> ShowNewArticlesByTag
        (string tagName, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //M20
    Task<Result<PaginatedList<ArticleBreifResponse>>> ShowNewArticlesByCategory
        (string categoryID, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //M21
    Task<Result<PaginatedList<ArticleBreifResponse>>> ShowNewArticlesByTitle
        (string title, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //M22
    Task<Result<PaginatedList<NewspaperUserResponse>>> SearchForNewspaper
        (string newspaperName, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //M23
    Task<Result<FullArticleFullResponse>> ReadArticle
        (string? memberID, string articleID, CancellationToken cancellationToken = default);

    //M24
    Task<Result<PaginatedList<CommentViewResponse>>> CommentsForArticle
        (string articleID, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //M25
    Task<Result<NewspaperUserResponse>> NewspaperProfile
        (string newspaperID, CancellationToken cancellationToken = default);

    //M26
    Task<Result<MemberUserResponse>> MemberProfile
        (string memberID, CancellationToken cancellationToken = default);

    //M27
    Task<Result<MemberUserResponse>> MyProfile
        (string memberID, CancellationToken cancellationToken = default);

    //M28
    Task<Result<PaginatedList<MemberUserResponse>>> ShowMembersLikes
        (string articleID, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //M29
    Task<Result<PaginatedList<MemberBreifResponseWithProfileImage>>> ShowMemberFollowing
        (string newspaperID, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //M30
    Task<Result<PaginatedList<ArticleBreifResponse>>> ArticleByNewspaper
        (string newspaperID, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //M31
    Task<Result<PaginatedList<CategoryResponse>>> GetAllCategories
        (ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //M32
    Task<Result<(string imagePath, string contentType)>> GetMyProfileImage
        (string membrID, CancellationToken cancellationToken = default);

    //M33
    Task<Result<(string imagePath, string contentType)>> GetMemberProfileImage
        (string membrID, CancellationToken cancellationToken = default);

    //M34
    Task<Result<(string imagePath, string contentType)>> GetNewspaperProfileImage
        (string membrID, CancellationToken cancellationToken = default);

    //M35
    Task<Result<(string imagePath, string contentType)>> GetArticleImage
        (string imageName, CancellationToken cancellationToken);

    //M36
    Task<Result<bool>> IsLiked
        (string memberID, string articleID, CancellationToken cancellationToken = default);

    //M37
    Task<Result<bool>> IsSaved
        (string memberID, string articleID, CancellationToken cancellationToken = default);

    //M38
    Task<Result<bool>> IsFollowed
        (string memberID, string newspaperID, CancellationToken cancellationToken = default);

    //M39
    Task<Result> ReportForNewspaper
        (string memberID, ReportForNewspaperRequest reportForNewspaperRequest, CancellationToken cancellationToken = default);

    //M40
    Task<Result> ReportForArticle
        (string memberID, ReportForArticleRequest reportForArticleRequest, CancellationToken cancellationToken = default);

    //M41
    Task<Result<PaginatedList<NotificationResponse>>> GetAllNotification
        (string memberID, bool isReaded, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //M42
    Task<Result> ReadAllNotifications
        (string memberID, CancellationToken cancellationToken = default);

    //M43
    Task<Result> ReadNotification
        (string memberID, string notificationID, CancellationToken cancellationToken = default);

    //M44
    Task<Result<int>> CountOfNotifications
        (string memberID, bool isReaded, CancellationToken cancellationToken = default);

}



