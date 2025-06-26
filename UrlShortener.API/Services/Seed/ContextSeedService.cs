using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UrlShortener.API.Data;
using UrlShortener.API.Models.Consts;
using UrlShortener.API.Models.Entities;

namespace UrlShortener.API.Services.Seed;

public class ContextSeedService
{
    private readonly UrlShortenerDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ContextSeedService(UrlShortenerDbContext dbContext,
        UserManager<User> userManager,
        IConfiguration configuration,
        RoleManager<IdentityRole> roleManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _configuration = configuration;
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

    public async Task SeedAdminUser()
    {
        string userName = _configuration.GetValue<string>("AdminAccountCredentials:UserName")!;
        string password = _configuration.GetValue<string>("AdminAccountCredentials:Password")!;

        if (await _userManager.FindByNameAsync(userName) is not null)
        {
            return;
        }

        User amdinUserToCreate = new()
        {
            UserName = userName,
        };

        var result = await _userManager.CreateAsync(amdinUserToCreate, password);

        if (result.Succeeded)
        {
            List<string> adminRoles = new List<string> { DbRolesConsts.MemberRole, DbRolesConsts.AdminRole };

            await _userManager.AddToRolesAsync(amdinUserToCreate, adminRoles);
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
