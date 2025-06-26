using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UrlShortener.API.Data;
using UrlShortener.API.Interfaces.Authentication;
using UrlShortener.API.Models.Consts;
using UrlShortener.API.Models.DTOs.Authentication;
using UrlShortener.API.Models.Entities;

namespace UrlShortener.API.Services.Authentication;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenHandler _tokenHandler;
    private readonly UrlShortenerDbContext _dbContext;

    public AuthService(UserManager<User> userManager, 
        RoleManager<IdentityRole> roleManager, 
        SignInManager<User> signInManager,
        ITokenHandler tokenHandler,
        UrlShortenerDbContext dbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _tokenHandler = tokenHandler;
        _dbContext = dbContext;
    }

    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var newUser = new User
        {
            UserName = request.UserName,
        };

        var result = await _userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded)
        {
            throw new Exception("User creation failed! Errors: " + string.Join(" ", result.Errors));
        }

        await _userManager.AddToRoleAsync(newUser, DbRolesConsts.MemberRole);

        var userRoles = await GetRolesForUserByUserId(newUser.Id);

        var response = new RegisterResponseDto()
        {
            Id = newUser.Id,
            Username = newUser.UserName,
            Roles = await GetRolesForUserByUserId(newUser.Id)!,
            Token = _tokenHandler.CreateAccessToken(newUser, userRoles),
        };

        return response;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.UserName!.Equals(request.UserName));

        if (user is null)
        {
            throw new ArgumentNullException("User is not found", nameof(user));
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

        if (!result.Succeeded)
        {
            throw new Exception("Login failed! Errors: " + string.Join(" ", result.ToString()));
        }

        var userRoles = await GetRolesForUserByUserId(user.Id);

        var response = new LoginResponseDto()
        {
            Id = user.Id,
            Username = user.UserName!,
            Token = _tokenHandler.CreateAccessToken(user, userRoles),
            Roles = await GetRolesForUserByUserId(user.Id)
        };

        return response;
    }

    private async Task<List<string>> GetRolesForUserByUserId(string userId)
    {
        User? user = await _userManager.FindByIdAsync(userId);

        if (user != null)
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);

            return roles.ToList();
        }

        return null!;
    }

}
