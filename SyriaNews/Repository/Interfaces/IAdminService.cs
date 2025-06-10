namespace SyriaNews.Repository.Interfaces;

public interface IAdminService
{
    //A1
    Task<Result<bool>> ToggleNewspaperStatus
        (string newpaperID, CancellationToken cancellationToken = default);

    //A2
    Task<Result<bool>> ToggleMemberStatus
        (string memberID, CancellationToken cancellationToken = default);

    //A3
    Task<Result<bool>> TogglePostStatus
        (string articleID, CancellationToken cancellationToken = default);

    //A4
    Task<Result> ConfirmUserAccount
        (string userID, UserTypes userTypes, CancellationToken cancellationToken = default);

    //A5
    Task<Result<PaginatedList<ArticleBreifResponse>>> SeeNewArticles
        (bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //A6
    Task<Result<PaginatedList<ArticleBreifResponse>>> SeeNewArticlesByCategory
        (string categoryID, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //A7
    Task<Result<PaginatedList<ArticleBreifResponse>>> SeeNewArticlesByTag
        (string tagName, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //A8
    Task<Result<PaginatedList<ArticleBreifResponse>>> SeeNewArticlesTitle
        (string title, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //A9
    Task<Result<PaginatedList<ArticleBreifResponse>>> SeeNewArticlesByViews
        (bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //A10
    Task<Result<PaginatedList<NewspaperUserResponse>>> SearchForNewspaperByName
        (string newspaperName, bool isActive, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //A11
    Task<Result<NewspaperUserResponse>> SeeNewspaperProfile
        (string newspaperID, bool isActive, CancellationToken cancellationToken = default);

    //A12
    Task<Result<PaginatedList<ArticleBreifResponse>>> SeeArticlesForNewspaper
        (string newspaperID, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //A13
    Task<Result<PaginatedList<CommentViewResponse>>> CommentsForArticle
        (string articleID, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //A14
    Task<Result<PaginatedList<MemberUserResponse>>> ShowArticlesLikes
        (string articleID, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //A15
    Task<Result<PaginatedList<FullCommentResponse>>> MemberComments
        (string memberID, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //A16
    Task<Result<PaginatedList<NewspaperUserResponse>>> MemeberFollows
        (string memberID, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //A17
    Task<Result<PaginatedList<FullSaveResponse>>> MemeberSaves
        (string memberID, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //A18
    Task<Result<PaginatedList<ArticleBreifResponse>>> MemeberLikes
        (string memberID, bool isActive, bool isPosted, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //A19
    Task<Result<MemberUserResponse>> SeeMemberProfile
        (string memberID, bool isActive, CancellationToken cancellationToken = default);

    //A20
    Task<Result> RemoveComment
        (string commentID, CancellationToken cancellationToken = default);

    //A21
    Task<Result<FullArticleFullResponse>> ReadArticle
        (string userAdminID, string articleID, bool isPosted, CancellationToken cancellationToken = default);

    //A22
    Task<Result<CategoryResponse>> AddCategory
        (CategoryRequest categoryRequest, CancellationToken cancellationToken = default);

    //A23
    Task<Result> UnpublishArticlesByCategory
        (string categoryID, CancellationToken cancellationToken = default);

    //A24
    Task<Result<TagResponse>> AddTag
        (TagRequest tagRequest, CancellationToken cancellationToken = default);

    //A25
    Task<Result<PaginatedList<CategoryResponse>>> GetAllCategories
        (ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //A26
    Task<Result<(string imagePath, string contentType)>> GetProfileImage
        (string userID, UserTypes userTypes, bool isActive, CancellationToken cancellationToken = default);

    //A27
    Task<Result<(string imagePath, string contentType)>> GetArticleImage
        (string imageName, bool isPosted, CancellationToken cancellationToken);

    //A28
    Task<Result<RoleResponse>> AddRole
        (RoleRequest roleRequest, CancellationToken cancellationToken = default);

    //A29
    Task<Result<PaginatedList<RoleResponse>>> GetAllRoles
        (bool isDeleted, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //A30
    Task<Result<RoleResponse>> GetRoleByID
        (string id, bool isDeleted = true, CancellationToken cancellationToken = default);

    //A31
    Task<Result> DeleteRole
        (string id, CancellationToken cancellationToken = default);

    //A32
    Task<Result<PaginatedList<NotificationResponse>>> GetAllNotifications
        (string userID, bool isReaded, ResultFilter resultFilter, CancellationToken cancellationToken = default);

    //A33
    Task<Result> ReadNotification
        (string userID, string notificationID, CancellationToken cancellationToken = default);

    //A34
    Task<Result> ReadAllNotifications
        (string userID, CancellationToken cancellationToken = default);

    //A35
    Task<Result<int>> CountOfNotifications
        (string userID, bool isReaded = true, CancellationToken cancellationToken = default);


}

