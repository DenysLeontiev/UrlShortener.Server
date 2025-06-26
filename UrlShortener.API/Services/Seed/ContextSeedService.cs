using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UrlShortener.API.Data;
using UrlShortener.API.Models.Consts;

namespace UrlShortener.API.Services.Seed;

public class ContextSeedService
{
    private readonly UrlShortenerDbContext _dbContext;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ContextSeedService(UrlShortenerDbContext dbContext, 
        RoleManager<IdentityRole> roleManager)
    {
        _dbContext = dbContext;
        _roleManager = roleManager;
    }

    public async Task ApplyPendingMigrationsAsync()
    {
        var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Count() > 0)
        {
            await _dbContext.Database.MigrateAsync();
        }
    }

    public async Task SeedUserRolesAsync()
    {
        if (!await _roleManager.Roles.AnyAsync())
        {
            await _roleManager.CreateAsync(new IdentityRole { Name = DbRolesConsts.AdminRole });
            await _roleManager.CreateAsync(new IdentityRole { Name = DbRolesConsts.MemberRole });
        }
    }
}
