namespace SyriaNews.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = DefaultRoles.Member)]
[EnableRateLimiting("Concurrency")]
public class MembersController(IUnitOfWork unitOfWork): ControllerBase
{
    private readonly IUnitOfWork unitOfWork = unitOfWork;

    [HttpGet("articles/saved")] //M1
    public async Task<IActionResult> FullSavedArtciels(
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.FullSavedArtciels(User.GetUserId()!, true, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpGet("articles/liked")]     //M2
    public async Task<IActionResult> LikedArticle(
        [FromQuery] ResultFilter resultFilter, 
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.LikedArticle(User.GetUserId()!, true, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpGet("followed")]   //M3
    public async Task<IActionResult> FollowedNewspapers(
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.FollowedNewspapers(User.GetUserId()!, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpPost("articles/comments")]     //M4
    public async Task<IActionResult> PostComment(
        [FromBody] CommentRequest commentRequest,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.CommentOnArticle(User.GetUserId()!, commentRequest, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpPost("articles/likes")]    //M5
    public async Task<IActionResult> LikeArticle(
        [FromBody] LikeRequest likeRequest,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.LikeArticle(User.GetUserId()!, likeRequest, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpPost("followed")]     //M6
    public async Task<IActionResult> FollowingNewspaper(
        [FromBody] FollowerRequest followerRequest,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.FollowingNewspaper(User.GetUserId()!, followerRequest, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpPut("articles/comments/{commentID}")]  //M7
    public async Task<IActionResult> UpdateComment(
        [FromRoute] string commentID,
        [FromBody] CommentUpdateRequest commentUpdateRequest,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.UpdateComment(User.GetUserId()!, commentID, commentUpdateRequest, cancellationToken);
        if (temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }

    [HttpDelete("articles/comments/{commentID}")]  //M8
    public async Task<IActionResult> RemoveComment(
        [FromRoute] string commentID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.RemoveComment(User.GetUserId()!, commentID, cancellationToken);
        if (temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }

    [HttpDelete("articles/{articleID}/likes")]     //M9
    public async Task<IActionResult> RemoveLike(
        [FromRoute] string articleID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.RemoveLike(User.GetUserId()!, articleID, cancellationToken);
        if (temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }

    [HttpDelete("followed/{newspaperID}")]     //M10
    public async Task<IActionResult> RemoveFollow(
        [FromRoute] string newspaperID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.RemoveFollow(User.GetUserId()!, newspaperID, cancellationToken);
        if (temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }

    [HttpPost("articles/saved")]    //M11
    public async Task<IActionResult> SaveArticle(
        [FromBody] SaveRequest saveRequest,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.SaveArticle(User.GetUserId()!, saveRequest, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpDelete("articles/saved/{articleID}")]     //M12
    public async Task<IActionResult> UnsaveArticle(
        [FromRoute] string articleID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.UnsaveArticle(User.GetUserId()!, articleID, cancellationToken);
        if (temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }

    [HttpPut("me")]     //M13
    public async Task<IActionResult> UpdateMemberProfile(
        [FromBody] MemberRequest memberRequest,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.UpdateMemberProfile(User.GetUserId()!, memberRequest, cancellationToken);
        if (temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }
    
    [HttpGet("my-comments")]     //M14
    public async Task<IActionResult> MemberComments(
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.MemberComments(User.GetUserId()!, true, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }
    
    [HttpPut("me/profile-image")]     //M15
    public async Task<IActionResult> ChangeProfileImage(
        [FromForm] IFormFile formFile,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.ChangeProfileImage(User.GetUserId()!, formFile, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }
    
    [HttpDelete("me/profile-image")]     //M16
    public async Task<IActionResult> RemoveProfileImage(CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.RemoveProfileImage(User.GetUserId()!, cancellationToken);
        if (temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }

    [AllowAnonymous]
    [HttpGet("articles/by-post-date")]       //M17
    public async Task<IActionResult> ShowNewArticlesByPostDate(
        [FromQuery]ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.ShowNewArticlesByPostDate(resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [AllowAnonymous]
    [HttpGet("articles/by-most-viewed")]       //M18
    public async Task<IActionResult> ShowNewArticlesByMostViewed(
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.ShowNewArticlesByMostViewed(resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [AllowAnonymous]
    [HttpGet("articles/by-tag/{tagName}")]       //M19
    public async Task<IActionResult> ShowNewArticlesByTag(
        [FromRoute] string tagName,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.ShowNewArticlesByTag(tagName, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [AllowAnonymous]
    [HttpGet("articles/by-category/{categoryID}")]       //M20
    public async Task<IActionResult> ShowNewArticlesByCategory(
        [FromRoute] string categoryID,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.ShowNewArticlesByCategory(categoryID, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [AllowAnonymous]
    [HttpGet("articles/by-title/{title}")]       //M21
    public async Task<IActionResult> ShowNewArticlesByTitle(
        [FromRoute] string title,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.ShowNewArticlesByTitle(title, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [AllowAnonymous]
    [HttpGet("search-newspaper/{newspaperName}")]       //M22
    public async Task<IActionResult> SearchForNewspaper(
        [FromRoute] string newspaperName,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.SearchForNewspaper(newspaperName, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [AllowAnonymous]
    [HttpGet("articles/{articleID}")]       //M23
    public async Task<IActionResult> ReadArticle(
        [FromRoute] string articleID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.ReadArticle(User.GetUserId(), articleID, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [AllowAnonymous]
    [HttpGet("articles/{articleID}/comments")]       //M24
    public async Task<IActionResult> CommentsForArticle(
        [FromRoute] string articleID,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.CommentsForArticle(articleID, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [AllowAnonymous]
    [HttpGet("newspaper/{newspaperID}")]       //M25
    public async Task<IActionResult> NewspaperProfile(
        [FromRoute] string newspaperID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.NewspaperProfile(newspaperID, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [AllowAnonymous]
    [HttpGet("{memberID}")]       //M26
    public async Task<IActionResult> MemberProfile(
        [FromRoute] string memberID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.MemberProfile(memberID, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }    

    [HttpGet("me")]       //M27
    public async Task<IActionResult> MyProfile(CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.MyProfile(User.GetUserId()!, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [AllowAnonymous]
    [HttpGet("articles/{articleID}/likes")]      //M28
    public async Task<IActionResult> ShowMembersLikes(
        [FromRoute]string articleID,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.ShowMembersLikes(articleID, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [AllowAnonymous]
    [HttpGet("newspaper/{newspaperID}/followers")]      //M29
    public async Task<IActionResult> ShowMemberFollowing(
        [FromRoute]string newspaperID,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.ShowMemberFollowing(newspaperID, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }
    
    [AllowAnonymous]
    [HttpGet("newspaper/{newspaperID}/articles")]      //M30
    public async Task<IActionResult> ArticleByNewspaper(
        [FromRoute]string newspaperID,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.ArticleByNewspaper(newspaperID, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [AllowAnonymous]
    [HttpGet("categories")]     //M31
    public async Task<IActionResult> GetAllCategories(
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.GetAllCategories(resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpPut("me/change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromBody]ChangePasswordRequest changePasswordRequest)
    {
        var temp = await unitOfWork.UserService.ChangePassword(User.GetUserId()!, changePasswordRequest);
        if (temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }

    [HttpGet("me/profile-image")]   //M32
    public async Task<IActionResult> GetMyProfileImage(CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.GetMyProfileImage(User.GetUserId()!, cancellationToken);
        if (temp.IsSuccess)
            return PhysicalFile(temp.Value.imagePath, temp.Value.contentType);
        return temp.ToProblem();
    }

    [AllowAnonymous]
    [HttpGet("members/{memberID}/profile-image")]   //M33
    public async Task<IActionResult> GetMemberProfileImage(
        [FromRoute]string memberID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.GetMemberProfileImage(memberID, cancellationToken);
        if (temp.IsSuccess)
            return PhysicalFile(temp.Value.imagePath, temp.Value.contentType);
        return temp.ToProblem();
    }

    [AllowAnonymous]
    [HttpGet("newspapers/{newspaperID}/profile-image")]   //M34
    public async Task<IActionResult> GetNewspaperProfileImage(
        [FromRoute]string newspaperID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.GetNewspaperProfileImage(newspaperID, cancellationToken);
        if (temp.IsSuccess)
            return PhysicalFile(temp.Value.imagePath, temp.Value.contentType);
        return temp.ToProblem();
    }

    [AllowAnonymous]
    [HttpGet("articles/images/{imageName}")]   //M35
    public async Task<IActionResult> GetArticleImage(
    [FromRoute] string imageName,
    CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.GetArticleImage(imageName, cancellationToken);
        if (temp.IsSuccess)
            return PhysicalFile(temp.Value.imagePath, temp.Value.contentType);
        return temp.ToProblem();
    }

    [HttpGet("articles/{articleID}/is-liked")]  //M36
    public async Task<IActionResult> IsLiked(
        [FromRoute]string articleID,
        CancellationToken cancellationToken = default
        )
    {
        var temp = await unitOfWork.MemberService.IsLiked(User.GetUserId()!, articleID, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpGet("articles/{articleID}/is-saved")]  //M37
    public async Task<IActionResult> IsSaved(
        [FromRoute]string articleID,
        CancellationToken cancellationToken = default
        )
    {
        var temp = await unitOfWork.MemberService.IsSaved(User.GetUserId()!, articleID, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpGet("newspapers/{newspaperID}/is-followed")]   //M38
    public async Task<IActionResult> IsFollowed(
        [FromRoute]string newspaperID,
        CancellationToken cancellationToken = default
        )
    {
        var temp = await unitOfWork.MemberService.IsFollowed(User.GetUserId()!, newspaperID, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpPost("reports/report-for-newspaper")]      //M39
    public async Task<IActionResult> ReportForNewspaper(
        [FromBody]ReportForNewspaperRequest reportForNewspaperRequest,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.ReportForNewspaper(User.GetUserId()!, reportForNewspaperRequest, cancellationToken);
        if (temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }
    
    [HttpPost("reports/report-for-article")]        //M40
    public async Task<IActionResult> ReportForArticle(
        [FromBody]ReportForArticleRequest reportForArticleRequest,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.ReportForArticle(User.GetUserId()!, reportForArticleRequest, cancellationToken);
        if (temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }  
    
    [HttpGet("notifications/{isReaded}")]        //M41
    public async Task<IActionResult> GetAllNotifications(
        [FromRoute]bool isReaded,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.GetAllNotification(User.GetUserId()!, isReaded, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }

    [HttpPost("notifications")]         //M42
    public async Task<IActionResult> ReadAllNotifications(CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.ReadAllNotifications(User.GetUserId()!, cancellationToken);
        if(temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }

    [HttpPost("notifications/{notificationID}")]        //M43
    public async Task<IActionResult> ReadNotification(
        [FromRoute] string notificationID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.MemberService.ReadNotification(User.GetUserId()!, notificationID, cancellationToken);
        if(temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }

    [HttpGet("notifications/{isReaded}/count")]     //M44
    public async Task<IActionResult> CountOfNotifications(
        [FromRoute]bool isReaded,
        CancellationToken cancellationToken)
    {
        var temp = await unitOfWork.MemberService.CountOfNotifications(User.GetUserId()!, isReaded, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }
}
