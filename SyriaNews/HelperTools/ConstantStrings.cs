namespace SyriaNews.HelperTools;

public class ConstantStrings
{
    public static string Origin = "https://localhost:5000";

    public static string AllowedOrigin = "AllowedOrigins";

    public static (string Title, string Message) NewspaperPostNewArticle(string newspaperName)
        => new("New Article",
                $"{newspaperName} post a new article you may like");

    public static (string Title, string Message) MemberCommentOnArticle(string memberFullName)
        => new($"New Comment",
               $"{memberFullName} commented on your article");

    public static (string Title, string Message) MemberLikeArticle(string memberFullName)
        => new("New Like",
              $"{memberFullName} likes your article");

    public static (string Title, string Message) MemberFollowNewspaper(string memberFullName)
        => new("New Follow",
              $"{memberFullName} follow you");

    public static (string Title, string Message) BlockUser
        = new("Blocked",
              "your account is blocked");

}
