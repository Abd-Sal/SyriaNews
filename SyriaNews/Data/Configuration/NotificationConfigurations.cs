namespace SyriaNews.Data.Configuration;

public class NotificationConfigurations : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Title)
            .HasMaxLength(100);

        builder.Property(x => x.Message)
            .HasMaxLength(150);

        builder.Property(x => x.EntityID)
            .HasMaxLength(200);

        builder.Property(x => x.EntityType)
            .HasMaxLength(200);

        builder.Property(x => x.SenderUserID)
            .HasMaxLength(200);

        builder.Property(x => x.ReceiveUserID)
            .HasMaxLength(200);

        builder.Property(x => x.NotificationTypes)
            .HasMaxLength(200);

        builder.ToTable($"{nameof(Notification)}s");
    }
}





