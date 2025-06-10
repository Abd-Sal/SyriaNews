namespace SyriaNews.Data.Configuration;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(e => e.TypeUser)
            .HasMaxLength(75);

        builder
            .OwnsMany(x => x.refreshTokens)
            .ToTable("RefreshTokens")
            .WithOwner()
            .HasForeignKey("UserID");

        var passwordHasher = new PasswordHasher<ApplicationUser>();

        builder.HasData([
            new ApplicationUser{    //Admin
                Id = DefaultUsers.AdminID,
                TypeUser = UserTypes.Admin.ToString(),
                UserName = DefaultUsers.AdminEmail,
                Email = DefaultUsers.AdminEmail,
                NormalizedUserName = DefaultUsers.AdminEmail.ToUpper(),
                NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
                SecurityStamp = DefaultUsers.AdminSecurityStamp,
                ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
                EmailConfirmed = true,
                JoinAt = new DateTime(year: 2025, month: 1, day: 1),
                PasswordHash = passwordHasher.HashPassword(null!, DefaultUsers.AdminPassword)
            },
            new ApplicationUser{    //Newspaper
                Id = DefaultUsers.NewspaperID,
                TypeUser = UserTypes.NewsPaper.ToString(),
                UserName = DefaultUsers.NewspaperEmail,
                Email = DefaultUsers.NewspaperEmail,
                NormalizedUserName = DefaultUsers.NewspaperEmail.ToUpper(),
                NormalizedEmail = DefaultUsers.NewspaperEmail.ToUpper(),
                SecurityStamp = DefaultUsers.NewspaperSecurityStamp,
                ConcurrencyStamp = DefaultUsers.NewspaperConcurrencyStamp,
                EmailConfirmed = true,
                JoinAt = new DateTime(year: 2025, month: 1, day: 1),
                PasswordHash = passwordHasher.HashPassword(null!, DefaultUsers.NewspaperPassword)
            },
            new ApplicationUser{    //Member
                Id = DefaultUsers.MemberID,
                TypeUser = UserTypes.Member.ToString(),
                UserName = DefaultUsers.MemberEmail,
                Email = DefaultUsers.MemberEmail,
                NormalizedUserName = DefaultUsers.MemberEmail.ToUpper(),
                NormalizedEmail = DefaultUsers.MemberEmail.ToUpper(),
                SecurityStamp = DefaultUsers.MemberSecurityStamp,
                ConcurrencyStamp = DefaultUsers.MemberConcurrencyStamp,
                EmailConfirmed = true,
                JoinAt = new DateTime(year: 2025, month: 1, day: 1),
                PasswordHash = passwordHasher.HashPassword(null!, DefaultUsers.MemberPassword)
            }
        ]);
    }
}
