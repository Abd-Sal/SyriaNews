namespace SyriaNews.Repository.Interfaces;

public interface INewsPaperService
{

    //N1
    Task<Result<FullArticleFullResponse>> PostFullArticle
        (string newsapperID, AddFullArticleRequest addFullArticleRequest, CancellationToken cancellationToken = default);

    //N2
    Task<Result<List<ImageResponse>>> SetImagesForArticle
            (string newspaperID, string articleID, List<FullImageRequest> fullImageRequests, CancellationToken cancellationToken = default);
    //N3
    Task<Result<List<TagResponse>>> SetTagForArticle
        (string newspaperID, string articleID, List<TagRequest> tagRequests, CancellationToken cancellationToken = default);

    //N4
    Task<Result> UpdateArticle
        (string newspaperID, string articleID, ArticleRequest articleRequest, CancellationToken cancellationToken = default);

    //N5
    Task<Result<bool>> TogglePostStatus
        (string? newspaperID, string articleID, CancellationToken cancellationToken = default);

    //N6
    Task<Result<int>> CalculateLikesForCategory
        (string newspaperID, string categoryID, CancellationToken cancellationToken = default);

    //N7
    Task<Result<int>> CalculateViewsForCategory
        (string newspaperID, string categoryID, CancellationToken cancellationToken = default);

    //N8
    Task<Result<PaginatedList<CategoryWithCountOfUseResponse>>> CategoryUsed
        (string newspaperID, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //N9
    Task<Result<int>> CountOfLikes
        (string newspaperID, CancellationToken cancellationToken = default);

    //N10
    Task<Result<int>> CountOfViews
        (string newspaperID, CancellationToken cancellationToken = default);

    //N11
    Task<Result<int>> CountOfPublishedArticles
        (string newspaperID, CancellationToken cancellationToken = default);

    //N12
    Task<Result<PaginatedList<TagResponse>>> UsedTagsForNewspaper
        (string newspaperID, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //N13
    Task<Result> UpdateNewspaper
        (string newspaperID, NewsPaperRequest newsPaperRequest, CancellationToken cancellationToken = default);

    //N14
    Task<Result<ProfileImageResponse>> ChangeProfileImage
        (string newspaperID, IFormFile formFile, CancellationToken cancellationToken = default);

    //N15
    Task<Result> RemoveProfileImage
        (string newspaperID, CancellationToken cancellationToken = default);

    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////Visistor Services//////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////////////////////////////////////

    //N16
    Task<Result<PaginatedList<ArticleBreifResponse>>> MyArticles
        (string newspaperID, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //N17
    Task<Result<PaginatedList<MemberUserResponse>>> ShowMembersLikes
        (string newspaperID, string articleID, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //N18
    Task<Result<MemberUserResponse>> MemberProfile
        (string memberID, CancellationToken cancellationToken = default);

    //N19
    Task<Result<NewspaperUserResponse>> MyProfile
        (string newspaperID, CancellationToken cancellationToken = default);

    //N20
    Task<Result<PaginatedList<CommentViewResponse>>> CommentsForArticle
        (string newspaperID, string articleID, ResultFilter resultFilter, CancellationToken cancellationToken = default);
    
    //N21
    Task<Result<FullArticleFullResponse>> ReadArticle
        (string newspaperID, string articleID, CancellationToken cancellationToken = default);

    //N22
    Task<Result<PaginatedList<CategoryResponse>>> GetAllCategories
        (ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //N23
    Task<Result<(string imagePath, string contentType)>> GetMyProfileImage
        (string newspaperID, CancellationToken cancellationToken = default);

    //N24
    Task<Result<(string imagePath, string contentType)>> GetMemberProfileImage
        (string memberID, CancellationToken cancellationToken);

    //N25
    Task<Result<(string imagePath, string contentType)>> GetArticleImage
        (string newspaperID, string imageName, CancellationToken cancellationToken);

    //N26
    Task<Result> ReportForMember
        (string newspaperID, ReportForMemberRequest reportForMemberRequest, CancellationToken cancellationToken = default);

    //N27
    Task<Result> ReportForAdmin
        (string newspaperID, AskForHelpServiceRequest askForHelpService, CancellationToken cancellationToken = default);

    //N28
    Task<Result<PaginatedList<NotificationResponse>>> GetAllNotification
        (string newspaperID, bool isReaded, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //N29
    Task<Result> ReadAllNotifications
        (string newspaperID, CancellationToken cancellationToken = default);

    //N30
    Task<Result> ReadNotification
        (string newspaperID, string notificationID, CancellationToken cancellationToken = default);

    //N31
    Task<Result<int>> CountOfNotifications
        (string newspaperID, bool isReaded, CancellationToken cancellationToken = default);

}
