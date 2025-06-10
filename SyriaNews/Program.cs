namespace SyriaNews;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDependencies(builder.Configuration);
        builder.Services.AddOpenApi();

        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration)
        );

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseHangfireDashboard("/jobs", new DashboardOptions
        {
            Authorization = [
                new HangfireCustomBasicAuthenticationFilter{
                    User = app.Configuration.GetValue<string>("HangfireSettings:Username"),
                    Pass = app.Configuration.GetValue<string>("HangfireSettings:Password")
                }
            ],
            DashboardTitle = "Syria News Dashboard"
        });

        app.UseSerilogRequestLogging();

        app.UseCors();

        app.UseAuthorization();

        app.MapControllers();

        app.UseStaticFiles();

        app.MapHealthChecks("health", new HealthCheckOptions { 
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.UseRateLimiter();
                
        app.Run();
    }
}
