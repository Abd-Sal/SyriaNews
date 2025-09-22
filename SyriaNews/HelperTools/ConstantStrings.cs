namespace SyriaNews.HelperTools;

public class ConstantStrings
{
    public readonly static string Origin = "https://localhost:5000";

    public readonly static string AllowedOrigin = "AllowedOrigins";

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

    public readonly static (string Title, string Message) BlockUser
        = new("Blocked",
              "your account is blocked");

    public static (string EmailSubject, string EmailBody) ConfirmationEmail(string name, string code, string id)
        => new(
            "✅ Syria News: Email Confirmation",
            File.ReadAllText("HelperTools\\EmailConfirmationBody.html")
                .Replace("[CLIENT_NAME]", name)
                .Replace("[VERIFICATION_URL]",
                $"{Origin}/auth/confirm-email?UserID={id}&Code={code}")
        );

    public static (string EmailSubject, string EmailBody) ChangePasswordEmail(string code)
        => new(
            "✅ Syria News: Change Password",
            File.ReadAllText("CodeEmailBody.html")
                .Replace("[CODE]", code)
        );

}
