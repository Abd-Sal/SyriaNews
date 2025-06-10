namespace SyriaNews.Data.Configuration;

public class ImageConfigurations : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.HasIndex(e => new { e.ArticleID, e.Placement }).IsUnique();

        builder.ToTable($"{nameof(Image)}s");
    }
}






