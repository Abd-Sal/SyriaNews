namespace SyriaNews.Data.Configuration;

public class MemberConfigurations : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.HasKey(e => e.UserID);
        builder.Property(e => e.UserID).ValueGeneratedNever();

        builder.Property(e => e.FirstName)
            .HasMaxLength(256);

        builder.Property(e => e.LastName)
            .HasMaxLength(256);

        builder.HasOne(x => x.User)
            .WithOne(x => x.Member)
            .HasForeignKey<Member>(x => x.UserID);

        builder.HasMany(x => x.Likes)
            .WithOne(x => x.Member)
            .HasForeignKey(x => x.MemberID);

        builder.HasMany(x => x.Comments)
            .WithOne(x => x.Member)
            .HasForeignKey(x => x.MemberID);

        builder.HasMany(x => x.Followers)
            .WithOne(x => x.Member)
            .HasForeignKey(x => x.MemberID);

        builder.HasMany(x => x.Saves)
            .WithOne(x => x.Member)
            .HasForeignKey(x => x.MemberID);

        builder.HasData([
            new Member{
                UserID = DefaultUsers.MemberID,
                FirstName = "Abd Al-Ruhman",
                LastName = "Al-Saleh",
                Gender = false,
                IsActive = true,
            }
        ]);

        builder.ToTable($"{nameof(Member)}s");
    }
}






