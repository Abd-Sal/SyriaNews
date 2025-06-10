namespace SyriaNews.Abstractions.Consts;

public static class DefaultRoles
{
    //Admin Roles
    public const string Admin = nameof(Admin);
    public const string AdminRoleID = "0ce39245-a57c-4d26-9343-79ed6611c233";
    public const string AdminRoleConcurrencyStamp = "9d9cdf99-989d-4d45-9e94-7adcc695c73c";

    //Member Roles
    public const string Member = nameof(Member);
    public const string MemberRoleID = "6134adcc-8a86-4343-909b-5d83c7b29d81";
    public const string MemberRoleConcurrencyStamp = "64e4c84b-cfd6-4f23-819a-a1053a613002";
    
    //Newspaper Roles
    public const string Newspaper = nameof(Newspaper);
    public const string NewspaperRoleID = "f297ee1c-65c5-45d7-8533-a24e36d0ec3a";
    public const string NewspaperRoleConcurrencyStamp = "d684b80c-6aad-48dc-bb9f-9b6c2d1d945b";

}

