namespace SyriaNews.Data.Configuration;

public class LikeConfigurations : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.HasIndex(e => new {e.MemberID, e.ArticleID}).IsUnique();

        builder.ToTable($"{nameof(Like)}s");
    }
}






