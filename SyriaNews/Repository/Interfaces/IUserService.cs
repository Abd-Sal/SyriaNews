namespace SyriaNews.Repository.Interfaces;

public interface IUserService
{
    public Task<Result<(object result, UserTypes userType)>> Me(string Id, CancellationToken cancellationToken = default);
    public Task<Result> ChangePassword(string Id, ChangePasswordRequest changePasswordRequest);
}
