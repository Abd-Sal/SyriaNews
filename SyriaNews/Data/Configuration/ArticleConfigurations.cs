namespace SyriaNews.Data.Configuration;

public class ArticleConfigurations : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(x => x.Title)
            .HasMaxLength(256);

        builder.Property(x => x.Descrpition)
            .HasMaxLength(500);

        builder.HasMany(x => x.Likes)
            .WithOne(x => x.Article)
            .HasForeignKey(x => x.ArticleID);

        builder.HasMany(x => x.ArticlesTags)
            .WithOne(x => x.Article)
            .HasForeignKey(x => x.ArticleID);

        builder.HasMany(x => x.Images)
            .WithOne(x => x.Article)
            .HasForeignKey(x => x.ArticleID);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Articles)
            .HasForeignKey(x => x.CategoryID);

        builder.HasOne(x => x.NewsPaper)
            .WithMany(x => x.Articles)
            .HasForeignKey(x => x.NewsPaperID);

        builder.HasMany(x => x.Saves)
            .WithOne(x => x.Article)
            .HasForeignKey(x => x.ArticleID);

        builder.ToTable($"{nameof(Article)}s");
    }
}






