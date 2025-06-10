namespace SyriaNews.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = DefaultRoles.Newspaper)]
[EnableRateLimiting("Concurrency")]
public class NewspapersController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork unitOfWork = unitOfWork;

    [HttpPost("articles")]  //N1
    public async Task<IActionResult> PostArticle
        ([FromForm] AddFullArticleRequest addFullArticleRequest,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.PostFullArticle(User.GetUserId()!, addFullArticleRequest, cancellationToken);
        if (temp.IsSuccess)
            return CreatedAtAction(nameof(GetArticle), new {articleID = temp.Value.Id }, temp.Value);
        return temp.ToProblem();
    }

    [HttpPost("articles/{articleID}/set-images")]      //N2
    public async Task<IActionResult> SetImagesForArticle(
        [FromRoute] string articleID,
        [FromForm] List<FullImageRequest> fullImageRequests,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.SetImagesForArticle(User.GetUserId()!, articleID, fullImageRequests, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }
    
    [HttpPost("articles/{articleID}/set-tags")]      //N3
    public async Task<IActionResult> SetTagForArticle(
        [FromRoute] string articleID,
        [FromForm] List<TagRequest> tagRequests,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.SetTagForArticle(User.GetUserId()!, articleID, tagRequests, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpPut("articles/{articleID}")]      //N4
    public async Task<IActionResult> UpdateArticle(
        [FromRoute] string articleID,
        [FromBody] ArticleRequest articleRequest,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.UpdateArticle(User.GetUserId()!, articleID, articleRequest, cancellationToken);
        if (temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }
    
    [HttpPut("articles/{articleID}/toggle-status")]      //N5
    public async Task<IActionResult> TogglePostStatus(
        [FromRoute] string articleID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.TogglePostStatus(User.GetUserId()!, articleID, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }
    
    [HttpGet("categories/{categoryID}/calculate-all-likes")]      //N6
    public async Task<IActionResult> CalculateLikesForCategory(
        [FromRoute] string categoryID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.CalculateLikesForCategory(User.GetUserId()!, categoryID, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }
        
    [HttpGet("categories/{categoryID}/calculate-all-views")]      //N7
    public async Task<IActionResult> CalculateViewsForCategory(
        [FromRoute] string categoryID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.CalculateViewsForCategory(User.GetUserId()!, categoryID, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }
            
    [HttpGet("categories/used-categories")]      //N8
    public async Task<IActionResult> CategoryUsed(
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.CategoryUsed(User.GetUserId()!, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }
            
    [HttpGet("count-of-likes")]      //N9
    public async Task<IActionResult> CountOfLikes(CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.CountOfLikes(User.GetUserId()!, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }
           
    [HttpGet("count-of-views")]      //N10
    public async Task<IActionResult> CountOfViews(CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.CountOfViews(User.GetUserId()!, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }
           
    [HttpGet("count-of-articles")]      //N11
    public async Task<IActionResult> CountOfPublishedArticles(CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.CountOfPublishedArticles(User.GetUserId()!, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpGet("tags/used-tags")]      //N12
    public async Task<IActionResult> UsedTagsForNewspaper(
        [FromQuery] ResultFilter resultFilter, 
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.UsedTagsForNewspaper(User.GetUserId()!, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }
           
    [HttpPut("me")]      //N13
    public async Task<IActionResult> UpdateNewspaper(
        [FromBody]NewsPaperRequest newsPaperRequest,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.UpdateNewspaper(User.GetUserId()!, newsPaperRequest, cancellationToken);
        if (temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }
               
    [HttpPut("me/profile-image")]      //N14
    public async Task<IActionResult> ChangeProfileImage(
        [FromForm]IFormFile formFile,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.ChangeProfileImage(User.GetUserId()!, formFile, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }
                   
    [HttpDelete("me/profile-image")]      //N15
    public async Task<IActionResult> RemoveProfileImage(
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.RemoveProfileImage(User.GetUserId()!, cancellationToken);
        if (temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }
                   
    [HttpGet("articles")]      //N16
    public async Task<IActionResult> MyArticles(
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.MyArticles(User.GetUserId()!, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }
                       
    [HttpGet("articles/{articleID}/likes")]      //N17
    public async Task<IActionResult> ShowMembersLikes(
        [FromRoute]string articleID,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.ShowMembersLikes(User.GetUserId()!, articleID, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }
        
    [HttpGet("visit-member-profile/{memberID}")]      //N18
    public async Task<IActionResult> MemberProfile(
        [FromRoute]string memberID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.MemberProfile(memberID, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }
        
    [HttpGet("me")]      //N19
    public async Task<IActionResult> MyProfile(CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.MyProfile(User.GetUserId()!, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }
        
    [HttpGet("articles/{articleID}/comments")]      //N20
    public async Task<IActionResult> CommentsForArticle(
        [FromRoute]string articleID,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.CommentsForArticle(User.GetUserId()!, articleID, resultFilter, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpGet("articles/{articleID}")]   //N21
    public async Task<IActionResult> GetArticle
        ([FromRoute] string articleID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.ReadArticle(User.GetUserId()!, articleID, cancellationToken);
        if (temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpGet("categories")]     //N22
    public async Task<IActionResult> GetAllCategories(
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.GetAllCategories(resultFilter, cancellationToken);
        if(temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpPut("me/change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest changePasswordRequest)
    {
        var temp = await unitOfWork.UserService.ChangePassword(User.GetUserId()!, changePasswordRequest);
        if (temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }

    [HttpGet("me/profile-image")]   //N23
    public async Task<IActionResult> GetMyProfileImage
        (CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.GetMyProfileImage(User.GetUserId()!, cancellationToken);
        if (temp.IsSuccess)
            return PhysicalFile(temp.Value.imagePath, temp.Value.contentType);
        return temp.ToProblem();
    }

    [HttpGet("members/{memberID}/profile-image")]    //N24
    public async Task<IActionResult> GetMemberProfileImage(
        [FromRoute]string memberID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.GetMemberProfileImage(memberID, cancellationToken);
        if (temp.IsSuccess)
            return PhysicalFile(temp.Value.imagePath, temp.Value.contentType);
        return temp.ToProblem();
    }
    
    [HttpGet("articles/images/{imageName}")]   //N25
    public async Task<IActionResult> GetArticleImage(
        [FromRoute]string imageName,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.GetArticleImage(User.GetUserId()!, imageName, cancellationToken);
        if (temp.IsSuccess)
            return PhysicalFile(temp.Value.imagePath, temp.Value.contentType);
        return temp.ToProblem();
    }

    [HttpPost("reports/report-for-member")]     //N26
    public async Task<IActionResult> ReportForMember(
        [FromBody] ReportForMemberRequest reportForMemberRequest,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.ReportForMember(User.GetUserId()!, reportForMemberRequest, cancellationToken);
        if(temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }

    [HttpPost("reports/report-for-ask-service")]     //N27
    public async Task<IActionResult> ReportForAdmin(
        [FromBody] AskForHelpServiceRequest askForHelpService,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.ReportForAdmin(User.GetUserId()!, askForHelpService, cancellationToken);
        if(temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }

    [HttpGet("notifications/{isReaded}")]       //N28
    public async Task<IActionResult> GetAllNotification(
        [FromRoute]bool isReaded,
        [FromQuery] ResultFilter resultFilter,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.GetAllNotification(User.GetUserId()!, isReaded, resultFilter, cancellationToken);
        if(temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

    [HttpPost("notifications")]      //N29
    public async Task<IActionResult> ReadAllNotifications(CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.ReadAllNotifications(User.GetUserId()!, cancellationToken);
        if(temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }
    
    [HttpPost("notifications/{notificationID}")]      //N30
    public async Task<IActionResult> ReadNotification(
        [FromRoute]string notificationID,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.ReadNotification(User.GetUserId()!, notificationID, cancellationToken);
        if(temp.IsSuccess)
            return NoContent();
        return temp.ToProblem();
    }
        
    [HttpGet("notifications/{isReaded}/count")]      //N31
    public async Task<IActionResult> CountOfNotifications(
        [FromRoute]bool isReaded,
        CancellationToken cancellationToken = default)
    {
        var temp = await unitOfWork.NewsPaperService.CountOfNotifications(User.GetUserId()!, isReaded, cancellationToken);
        if(temp.IsSuccess)
            return Ok(temp.Value);
        return temp.ToProblem();
    }

}
