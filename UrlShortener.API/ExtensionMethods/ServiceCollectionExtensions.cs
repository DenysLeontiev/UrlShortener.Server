using UrlShortener.API.Interfaces.Authentication;
using UrlShortener.API.Interfaces.Persistence;
using UrlShortener.API.Interfaces.Urls;
using UrlShortener.API.Repositories;
using UrlShortener.API.Services.Authentication;
using UrlShortener.API.Services.Seed;

namespace UrlShortener.API.ExtensionMethods;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenHandler, TokenHandler>();
        services.AddScoped<ContextSeedService>();
        services.AddScoped<IUrlRepository, UrlRepository>();
        services.AddScoped<UrlShorteningService>();
        services.AddAutoMapper(typeof(Program));
        services.AddHttpContextAccessor();
        services.ConfigureApplicationCookie(options =>
        {
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
            options.Events.OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
        });
        return services;
    }
}
