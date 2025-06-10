namespace SyriaNews.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) :
    IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
{

    public DbSet<Admin> Admins { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<ArticlesTags> ArticlesTags { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Follower> Followers { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<NewsPaper> NewsPapers { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Save> Saves { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ProfileImage> ProfileImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        var _FKs = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade &&
                !fk.IsOwnership);

        foreach (var fk in _FKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }
}
