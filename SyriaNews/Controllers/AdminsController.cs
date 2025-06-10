namespace SyriaNews.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = DefaultRoles.Admin)]
public class AdminsController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork unitOfWork = unitOfWork;

    [HttpPut("newspapers/toggle-status/{newspaperID}")]     //A1
    public async Task<IActionResult> NewspaperToggleStatus
        ([FromRoute] string newpaperID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.ToggleNewspaperStatus(newpaperID, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpPut("members/toggle-status/{memberID}")]     //A2
    public async Task<IActionResult> MemberToggleStatus
        ([FromRoute] string memberID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.ToggleMemberStatus(memberID, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpPut("newspapers/articles/toggle-status/{articleID}")]     //A3
    public async Task<IActionResult> ArticleToggleStatus
        ([FromRoute] string articleID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.TogglePostStatus(articleID, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpPut("newspapers/{newspaperID}/confirm-manually")]     //A4
    public async Task<IActionResult> ConfirmNewspaperAccount
        ([FromRoute] string newspaperID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.ConfirmUserAccount(newspaperID, UserTypes.NewsPaper, cancellationToken);
        if (temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }

    [HttpGet("newspapers/articles/{accordingToPosted}")]     //A5
    public async Task<IActionResult> SeeNewArticles
        ([FromRoute] bool accordingToPosted,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.SeeNewArticles(accordingToPosted, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpGet("newspapers/articles/{accordingToPosted}/by-category/{categoryID}")]     //A6
    public async Task<IActionResult> SeeNewArticlesByCategory
        ([FromRoute] bool accordingToPosted,
        [FromRoute] string categoryID,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.SeeNewArticlesByCategory(categoryID, accordingToPosted, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpGet("newspapers/articles/{accordingToPosted}/by-tag/{tagID}")]     //A7
    public async Task<IActionResult> SeeNewArticlesByTag
        ([FromRoute] bool accordingToPosted,
        [FromRoute] string tagID,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.SeeNewArticlesByTag(tagID, accordingToPosted, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpGet("newspapers/articles/{accordingToPosted}/by-title")]     //A8
    public async Task<IActionResult> SeeNewArticlesByTitle
        ([FromRoute] bool accordingToPosted,
        [FromQuery(Name = "title")] string title,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.SeeNewArticlesTitle(title, accordingToPosted, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpGet("newspapers/articles/{accordingToPosted}/by-views")]     //A9
    public async Task<IActionResult> SeeNewArticlesByViews
        ([FromRoute] bool accordingToPosted,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.SeeNewArticlesByViews(accordingToPosted, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpGet("newspapers/{newspaperName}/{accordingToActive}")]     //A10
    public async Task<IActionResult> SearchForNewspaperByName
        ([FromRoute] bool accordingToActive,
        [FromRoute] string newspaperName,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.SearchForNewspaperByName(newspaperName, accordingToActive, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpGet("newspapers/{newspaperID}/{accordingToActive}/profile")]     //A11
    public async Task<IActionResult> SeeNewspaperProfile
        ([FromRoute] string newspaperID,
        [FromRoute] bool accordingToActive,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.SeeNewspaperProfile(newspaperID, accordingToActive, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpGet("newspapers/{newspaperID}/articles/{accordingToPost}")]     //A12
    public async Task<IActionResult> SeeArticlesForNewspaper
        ([FromRoute] string newspaperID,
        [FromRoute] bool accordingToPost,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.SeeArticlesForNewspaper(newspaperID, accordingToPost, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpGet("newspapers/articles/{articleID}/{accordingToPost}/comments")]     //A13
    public async Task<IActionResult> CommentsForArticle
        ([FromRoute] string articleID,
        [FromRoute] bool accordingToPost,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.CommentsForArticle(articleID, accordingToPost, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpGet("newspapers/articles/{articleID}/likes")]     //A14
    public async Task<IActionResult> ShowArticlesLikes
        ([FromRoute] string articleID,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.ShowArticlesLikes(articleID, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpGet("members/{memberID}/{accordingToActive}/comments")]     //A15
    public async Task<IActionResult> MemberComments
        ([FromRoute] string memberID,
        [FromRoute] bool accordingToActive,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.MemberComments(memberID, accordingToActive, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpGet("members/{memberID}/follows")]     //A16
    public async Task<IActionResult> MemeberFollows
        ([FromRoute] string memberID,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.MemeberFollows(memberID, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpGet("members/{memberID}/{accordingToActive}/{accordingToPost}/saves")]     //A17
    public async Task<IActionResult> MemeberSaves
        ([FromRoute] string memberID,
        [FromRoute] bool accordingToPost,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.MemeberSaves(memberID, accordingToPost, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpGet("members/{memberID}/{accordingToActive}/{accordingToPost}/likes")]     //A18
    public async Task<IActionResult> MemeberLikes
        ([FromRoute] string memberID,
        [FromRoute] bool accordingToActive,
        [FromRoute] bool accordingToPost,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.MemeberLikes(memberID, accordingToActive, accordingToPost, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpGet("members/{memberID}/{accordingToActive}/profile")]     //A19
    public async Task<IActionResult> SeeMemberProfile
        ([FromRoute] string memberID,
        [FromRoute] bool accordingToActive,
        [FromRoute] bool accordingToPost,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.SeeMemberProfile(memberID, accordingToActive, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpDelete("members/comments/{commentID}")]     //A20
    public async Task<IActionResult> RemoveComment
        ([FromRoute] string commentID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.RemoveComment(commentID, cancellationToken);
        if (temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }

    [HttpGet("newspaper/articles/{articleID}/{acorrdingIsPost}")]       //A21
    public async Task<IActionResult> ReadArticle
        ([FromRoute] string articleID,
        [FromRoute] bool acorrdingIsPost,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.ReadArticle(User.GetUserId()!, articleID, acorrdingIsPost, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpPost("categories")]        //A22
    public async Task<IActionResult> AddCategory
        ([FromBody]CategoryRequest categoryRequest,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.AddCategory(categoryRequest, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }
    
    [HttpPut("categories/{categoryID}/unpublish-articles-by-category")]     //A23
    public async Task<IActionResult> UnpublishByCategory
        ([FromRoute]string categoryID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.UnpublishArticlesByCategory(categoryID, cancellationToken);
        if (temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }

    [HttpPost("tags")]      //A24
    public async Task<IActionResult> AddTag
        (TagRequest tagRequest, CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.AddTag(tagRequest, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpGet("categories")]     //A25
    public async Task<IActionResult> GetAllCategories(
        [FromQuery]ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.GetAllCategories(resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpGet("newspapers/{newspaperID}/profile-image")]     //A26
    public async Task<IActionResult> GetNewspaperProfileImage(
        [FromRoute] string newspaperID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.GetProfileImage
            (newspaperID, UserTypes.NewsPaper, false, cancellationToken);
        if (temp.IsSuccess)
            return PhysicalFile(temp.Value.imagePath, temp.Value.contentType);
        return temp.ToProblem();
    }

    [HttpGet("members/{memberID}/profile-image")]       //A26
    public async Task<IActionResult> GetMemberProfileImage(
        [FromRoute] string memberID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.GetProfileImage
            (memberID, UserTypes.Member, false, cancellationToken);
        if (temp.IsSuccess)
            return PhysicalFile(temp.Value.imagePath, temp.Value.contentType);
        return temp.ToProblem();
    }

    [HttpGet("articles/images/{imageName}")]        //A27
    public async Task<IActionResult> GetArticleImage(
        [FromRoute] string imageName,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.GetArticleImage(imageName, false, cancellationToken);
        if (temp.IsSuccess)
            return PhysicalFile(temp.Value.imagePath, temp.Value.contentType);
        return temp.ToProblem();
    }

    [HttpGet("roles/{isDeleted}")]      //A28
    public async Task<IActionResult> GetAllRoles(
        [FromRoute]bool isDeleted,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.GetAllRoles(isDeleted, resultFilter, cancellationToken);
        if(temp.IsSuccess)
            return Ok(temp);
        return temp.ToProblem();
    }

    [HttpGet("roles/{roleID}/{isDeleted}")]     //A29
    public async Task<IActionResult> GetRoleByID(
        [FromRoute] string roleID,
        [FromRoute] bool isDeleted = true,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.GetRoleByID(roleID, isDeleted, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp);
        return temp.ToProblem();
    }

    [HttpPost("roles")]     //A30
    public async Task<IActionResult> AddRole(
        [FromBody] RoleRequest roleRequest,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.AddRole(roleRequest, cancellationToken);
        if (temp.IsSuccess)
            return CreatedAtAction(nameof(GetRoleByID), new { roleID = temp.Value.Id }, temp.Value);
        return temp.ToProblem();
    }

    [HttpDelete("roles/{roleID}")]      //A31
    public async Task<IActionResult> DeleteRole(
        [FromRoute] string roleID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.DeleteRole(roleID, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp);
        return temp.ToProblem();
    }

    [HttpGet("notifications/{isReaded}")]       //A32
    public async Task<IActionResult> GetAllNotifications(
        [FromRoute]bool isReaded,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.GetAllNotifications(User.GetUserId()!, isReaded, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpPost("notifications/{notificationID}")]     //A33
    public async Task<IActionResult> ReadNotification(
        [FromRoute]string notificationID, 
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.ReadNotification(User.GetUserId()!, notificationID, cancellationToken);
        if (temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }

    [HttpPost("notifications")]     //A34
    public async Task<IActionResult> ReadAllNotifications(CancellationToken cancellationToken = default)
    {
        var temp= await unitOfWork.AdminService.ReadAllNotifications(User.GetUserId()!, cancellationToken);
        if (temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }

    [HttpGet("notification/{isReaded}/count")]  //A35
    public async Task<IActionResult> CountOfNotifications(
        [FromRoute] bool isReaded = true,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.AdminService.CountOfNotifications(User.GetUserId()!, isReaded, cancellationToken);
        if(temp.IsSuccess) 
            return Ok(temp.Value);
        return temp.ToProblem();
    }

}
