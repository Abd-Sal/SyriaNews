namespace SyriaNews.Data.Configuration;

public class SaveConfigurations : IEntityTypeConfiguration<Save>
{
    public void Configure(EntityTypeBuilder<Save> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.HasIndex(e => new { e.MemberID, e.ArticleID }).IsUnique();

        builder.ToTable($"{nameof(Save)}s");
    }
}





