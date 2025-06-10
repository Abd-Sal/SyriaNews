namespace SyriaNews.UnitOfWork;

public interface IUnitOfWork
{
    IAuthService AuthService { get; }
    INewsPaperService NewsPaperService { get; }
    IAdminService AdminService { get; }
    IMemberService MemberService { get; }
    IUserService UserService { get; }
    IVisitorService VisitorService { get; }
}
