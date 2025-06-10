namespace SyriaNews.Data.Configuration;

public class AdminConfigurations : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder.HasKey(e => e.UserID);
        builder.Property(e => e.UserID).ValueGeneratedNever();

        builder.HasOne(x => x.User)
            .WithOne(x => x.Admin)
            .HasForeignKey<Admin>(x => x.UserID);

        builder.HasData([
            new Admin{
                UserID = DefaultUsers.AdminID,
            }
        ]);

        builder.ToTable($"{nameof(Admin)}s");
    }
}





