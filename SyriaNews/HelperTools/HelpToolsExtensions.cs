namespace SyriaNews.HelperTools;

public class HelpToolsExtensions
{
    public static bool PasswordFormat(string password)
        => password.Trim().Length >= 8 &&
           password.Any(c => "ABCDEFGHIJKLMNOPQRSTUVWXYZ".Contains(c)) &&
           password.Any(c => "abcdefghijklmnopqrstuvwxyz".Contains(c)) &&
           password.Any(c => "123456789".Contains(c)) &&
           password.Any(c => "*#&@!%$^()_-+=[]{}:;'\"\\/|<>?".Contains(c));

    public static async Task<bool> checkUserActive
        (string userID, UserTypes userTypes, AppDbContext appDbContext, CancellationToken cancellationToken = default)
    {
        if (userTypes == UserTypes.Member)
            return await appDbContext.Members.AnyAsync(x => x.UserID == userID && x.IsActive, cancellationToken);
        else if(userTypes == UserTypes.NewsPaper)
            return await appDbContext.NewsPapers.AnyAsync(x => x.UserID == userID && x.IsActive, cancellationToken);
        return false;
    }

}
