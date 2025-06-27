using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrlShortener.API.Data;
using UrlShortener.API.ExtensionMethods;
using UrlShortener.API.Interfaces.Persistence;
using UrlShortener.API.Models;
using UrlShortener.API.Repositories;
using UrlShortener.API.Services.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddCors();

builder.Services.AddSwaggerWithJwtSupport();

builder.Services.RegisterDbContext(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.RegisterApplicationServices();
builder.Services.AddIdentityServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(opt =>
{
    opt.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(builder.Configuration["JWT:ClientUrl"]!, builder.Configuration["JWT:Issuer"]!);
});

app.MapGet("{code}", async (string code, IUrlRepository urlRepository) =>
{
    var shortenedUrl = await urlRepository.GetOriginalUrlAsync(code);
    if (shortenedUrl == null)
    {
        return Results.NotFound();
    }
    return Results.Redirect(shortenedUrl.OriginalUrl);
});

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();

var contextSeedService = scope.ServiceProvider.GetRequiredService<ContextSeedService>();

await contextSeedService.ApplyPendingMigrationsAsync();

await contextSeedService.SeedUserRolesAsync();
await contextSeedService.SeedAdminUser();

await contextSeedService.SeedAboutPage();

app.Run();
