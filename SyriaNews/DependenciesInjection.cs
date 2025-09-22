namespace SyriaNews;

public static class DependenciesInjection
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
    {

        var connectionString = configuration.GetConnectionString("DefaultConnections") ??
            throw new InvalidOperationException("Connection string not found.");

        services.AddControllers();

        services.AddOptions<ArticleImages>()
            .BindConfiguration(ArticleImages.sectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<ProfileImages>()
            .BindConfiguration(ProfileImages.sectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<EmailConfigurationsOptions>()
            .Bind(configuration.GetSection(EmailConfigurationsOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services
            .AddDbContext(configuration)
            .AddAuthConfig(configuration)
            .AddServicesConfig()
            .AddAutoMapperConfig()
            .AddFluentValidationConfig()
            .AddGlobalExceptionHandler()
            .AddingCorsConfig(configuration)
            .AddingHybridCacheConfig()
            .AddBackgroundJobsConfig(configuration)
            .AddHealthCheckConfig(connectionString)
            .AddRateLimiterConfig();
        return services;
    }
    private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnections") ??
            throw new InvalidOperationException("connection string 'DefaultConnections' not found ");
        services.AddDbContext<AppDbContext>(option =>
            option.UseSqlServer(connectionString));
        return services;
    }
    private static IServiceCollection AddGlobalExceptionHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        return services;
    }
    private static IServiceCollection AddAutoMapperConfig(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(AutoMappingProfile).Assembly);
        return services;
    }
    private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        return services;
    }
    private static IServiceCollection AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(JwtOption.SectionName).Get<JwtOption>();

        services.AddOptions<JwtOption>()
            .BindConfiguration(JwtOption.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(o =>
        {
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions?.Key!)),
                ValidIssuer = jwtOptions?.Issuer,
                ValidAudience = jwtOptions?.Audience
            };
        });

        services.Configure<IdentityOptions>(option =>
        {
            option.Password.RequiredLength = 8;

            option.User.RequireUniqueEmail = true;
            option.SignIn.RequireConfirmedEmail = true;
        });

        return services;
    }
    private static IServiceCollection AddServicesConfig(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<INotificationSender, NotificationSender>();
        return services;
    }
    private static IServiceCollection AddingCorsConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(option =>
            option.AddDefaultPolicy(builder =>
                builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithOrigins(configuration.GetSection(ConstantStrings.AllowedOrigin).Get<string[]>()!)
            )
        );
        return services;
    }
    private static IServiceCollection AddingHybridCacheConfig(this IServiceCollection services)
    {
        services.AddHybridCache(options =>
        {
            options.MaximumPayloadBytes = 1024 * 1024 * 100;    //cahce size is 50MB
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(30),
                LocalCacheExpiration = TimeSpan.FromHours(6),
            };
        });
        return services;
    }
    private static IServiceCollection AddBackgroundJobsConfig(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration.GetConnectionString("SyriaNewsJobs")));
        services.AddHangfireServer();
        return services;
    }
    private static IServiceCollection AddHealthCheckConfig(this IServiceCollection services, string connectionString)
    {
        services
            .AddHealthChecks()
            .AddSqlServer(name: "Database", connectionString: connectionString)
            .AddHangfire(option =>
            {
                option.MinimumAvailableServers = 1;
            })
            ;
        return services;
    }
    private static IServiceCollection AddRateLimiterConfig(this IServiceCollection services)
    {
        services
            .AddRateLimiter(rateLimiterOptions =>
            {
                rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                rateLimiterOptions.AddConcurrencyLimiter("Concurrency", options =>
                {
                    options.PermitLimit = 1000;
                    options.QueueLimit = 100;
                    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });
            });
        return services;
    }

}
