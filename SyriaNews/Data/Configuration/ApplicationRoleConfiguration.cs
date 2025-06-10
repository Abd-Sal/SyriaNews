namespace SyriaNews.Data.Configuration;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.HasData([
            new ApplicationRole{    //Admin
                Id = DefaultRoles.AdminRoleID,
                Name = DefaultRoles.Admin,
                NormalizedName = DefaultRoles.Admin.ToUpper(),
                ConcurrencyStamp = DefaultRoles.AdminRoleConcurrencyStamp
            },
            new ApplicationRole{    //Newspaper
                Id = DefaultRoles.NewspaperRoleID,
                Name = DefaultRoles.Newspaper,
                NormalizedName = DefaultRoles.Newspaper.ToUpper(),
                ConcurrencyStamp = DefaultRoles.NewspaperRoleConcurrencyStamp,
                IsDefault = true
            },
            new ApplicationRole{    //Member
                Id = DefaultRoles.MemberRoleID,
                Name = DefaultRoles.Member,
                NormalizedName = DefaultRoles.Member.ToUpper(),
                ConcurrencyStamp = DefaultRoles.MemberRoleConcurrencyStamp,
                IsDefault = true
            }
        ]);
    }
}

