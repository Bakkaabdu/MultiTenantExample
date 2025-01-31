
using Microsoft.EntityFrameworkCore;

namespace MultiTenantExample;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        builder.Services.AddScoped<ITenantService, TenantService>();

        builder.Services.Configure<TenantSittings>(builder.Configuration.GetSection(nameof(TenantSittings)));

        TenantSittings options = new();
        builder.Configuration.GetSection(nameof(TenantSittings)).Bind(options);


        var defaultDbProvider = options.Defaults.DBProvider;

        if (defaultDbProvider.ToLower() == "mssql")
        {
            builder.Services.AddDbContext<ApplicationDbContext>(s => s.UseSqlServer());
        }

        foreach (var tenant in options.Tenants)
        {
            var connectionString = tenant.ConnectionString ?? options.Defaults.ConnectionString;
            using var scope = builder.Services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            context.Database.SetConnectionString(connectionString);

            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }
        }

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
