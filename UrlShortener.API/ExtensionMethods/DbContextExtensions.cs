using Microsoft.EntityFrameworkCore;
using UrlShortener.API.Data;

namespace UrlShortener.API.ExtensionMethods;

public static class DbContextExtensions
{
    public static IServiceCollection RegisterDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")!;

        services.AddDbContext<UrlShortenerDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        return services;
    }
}
