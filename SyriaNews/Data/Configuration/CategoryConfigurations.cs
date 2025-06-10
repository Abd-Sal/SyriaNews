namespace SyriaNews.Data.Configuration;

public class CategoryConfigurations : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.Property(e => e.CategoryName)
            .HasMaxLength(256);

        builder.HasIndex(e => e.CategoryName).IsUnique();

        builder.ToTable("Categories");
    }
}






