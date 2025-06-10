namespace SyriaNews.Data.Configuration;

public class FollowerConfigurations : IEntityTypeConfiguration<Follower>
{
    public void Configure(EntityTypeBuilder<Follower> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.HasIndex(e => new {e.MemberID, e.NewsPaperID}).IsUnique();

        builder.ToTable($"{nameof(Follower)}s");
    }
}






