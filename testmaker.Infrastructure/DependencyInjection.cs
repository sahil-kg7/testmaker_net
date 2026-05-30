using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using testmaker.Application.Common.Interfaces;
using testmaker.Infrastructure.Persistence;

namespace testmaker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var mySqlServerVersion = configuration["Database:MySqlServerVersion"];

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
        }

        if (!Version.TryParse(mySqlServerVersion, out var serverVersion))
        {
            throw new InvalidOperationException("Configuration value 'Database:MySqlServerVersion' must be a valid version, for example '8.0.36'.");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(
                connectionString,
                new MySqlServerVersion(serverVersion),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
            .AddInterceptors(new AuditSaveChangesInterceptor()));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}