namespace SyriaNews.Repository.Interfaces;

public interface IVisitorService
{
    //V1
    Task<Result> AddNewspaper
        (NewspaperAddRequest newspaperAddRequest, CancellationToken cancellationToken = default);

    //V2
    Task<Result> AddMember
        (MemberAddRequest memberAddRequest, CancellationToken cancellationToken = default);

    //V3
    Task<Result<PaginatedList<ArticleBreifResponse>>> ShowNewArticlesByPostDate
        (bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //V4
    Task<Result<PaginatedList<ArticleBreifResponse>>> ShowNewArticlesByMostViewed
        (bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //V5
    Task<Result<PaginatedList<ArticleBreifResponse>>> ShowNewArticlesByTag
        (string tagName, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //V6
    Task<Result<PaginatedList<ArticleBreifResponse>>> ShowNewArticlesByCategory
        (string categoryID, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //V7
    Task<Result<PaginatedList<ArticleBreifResponse>>> ShowNewArticlesByTitle
        (string title, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //V8
    Task<Result<PaginatedList<NewspaperUserResponse>>> SearchForNewspaper
        (string newspaperName, bool isActive, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //V9
    Task<Result<FullArticleFullResponse>> ReadArticle
        (string? userID, string articleID, bool isPosted = true, CancellationToken cancellationToken = default);

    //V10
    Task<Result<PaginatedList<CommentViewResponse>>> CommentsForArticle
        (string articleID, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //V11
    Task<Result<NewspaperUserResponse>> NewspaperProfile
        (string newspaperID, bool isActive = true, CancellationToken cancellationToken = default);

    //V12
    Task<Result<MemberUserResponse>> MemberProfile
        (string memberID, bool isActive = true, CancellationToken cancellationToken = default);

    //V13
    Task<Result<PaginatedList<MemberUserResponse>>> ShowMembersLikes
        (string? newspaperID, string articleID, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //V14
    Task<Result<PaginatedList<MemberBreifResponseWithProfileImage>>> ShowMemberFollowing
        (string newspaperID, bool isActive, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //V15
    Task<Result<PaginatedList<ArticleBreifResponse>>> ArticleByNewspaper
        (string newspaperID, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //V16
    Task<Result<PaginatedList<CategoryResponse>>> GetAllCategories
        (ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //V17
    Task<Result<(string imagePath, string contentType)>> GetProfileImage
        (string userID, UserTypes userTypes, bool isActive, CancellationToken cancellationToken = default);

    //V18
    Task<Result<(string imagePath, string contentType)>> GetArticleImage
        (string name, bool isPosted, CancellationToken cancellationToken = default);
}
