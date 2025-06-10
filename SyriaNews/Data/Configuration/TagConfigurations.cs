namespace SyriaNews.Data.Configuration;

public class TagConfigurations : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.TagName)
            .HasMaxLength(256);

        builder.HasMany(x => x.ArticlesTags)
            .WithOne(x => x.Tag)
            .HasForeignKey(x => x.TagID);

        builder.HasIndex(e => e.TagName).IsUnique();

        builder.ToTable($"{nameof(Tag)}s");
    }
}





