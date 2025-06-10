namespace SyriaNews.Data.Configuration;

public class CommentConfigurations : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.Content)
            .HasMaxLength(2000);

        builder.ToTable($"{nameof(Comment)}s");
    }
}






