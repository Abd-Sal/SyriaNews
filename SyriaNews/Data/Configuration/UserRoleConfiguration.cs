namespace SyriaNews.Data.Configuration;

public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        builder.HasData([
            new IdentityUserRole<string>{   //Admin
                UserId = DefaultUsers.AdminID,
                RoleId = DefaultRoles.AdminRoleID,
            },
            new IdentityUserRole<string>{   //Newspaper
                UserId = DefaultUsers.NewspaperID,
                RoleId = DefaultRoles.NewspaperRoleID,
            },
            new IdentityUserRole<string>{   //Member
                UserId = DefaultUsers.MemberID,
                RoleId = DefaultRoles.MemberRoleID,
            }
        ]);
    }
}

