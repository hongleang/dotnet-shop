using DotnetShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetShop.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException(
                "Connection string 'Default' is missing. Set it with: " +
                "dotnet user-secrets set \"ConnectionStrings:Default\" \"<value>\" --project src/DotnetShop.Api");

        // AddDbContext registers AppDbContext as Scoped: one instance per HTTP request.
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        return services;
    }
}
