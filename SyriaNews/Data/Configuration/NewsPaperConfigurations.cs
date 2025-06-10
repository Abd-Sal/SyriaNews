namespace SyriaNews.Data.Configuration;

public class NewsPaperConfigurations : IEntityTypeConfiguration<NewsPaper>
{
    public void Configure(EntityTypeBuilder<NewsPaper> builder)
    {
        builder.HasKey(e => e.UserID);
        builder.Property(e => e.UserID).ValueGeneratedNever();

        builder.Property(e => e.Name)
            .HasMaxLength(256);

        builder.HasOne(x => x.User)
            .WithOne(x => x.NewsPaper)
            .HasForeignKey<NewsPaper>(x => x.UserID);

        builder.HasMany(x => x.Followers)
            .WithOne(x => x.NewsPaper)
            .HasForeignKey(x => x.NewsPaperID);

        builder.HasIndex(e => e.Name).IsUnique();

        builder.HasData([
            new NewsPaper{
                UserID = DefaultUsers.NewspaperID,
                Name = "Sama-News",
                IsActive = true
            }
        ]);

        builder.ToTable($"{nameof(NewsPaper)}s");
    }
}





