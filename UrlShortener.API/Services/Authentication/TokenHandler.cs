using System.Security.Claims;
using System.Text;
using UrlShortener.API.Interfaces.Authentication;
using UrlShortener.API.Models.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace UrlShortener.API.Services.Authentication;

public class TokenHandler : ITokenHandler
{
    private readonly SymmetricSecurityKey _symmetricSecurityKey;
    private readonly IConfiguration _configuration;

    public TokenHandler(IConfiguration configuration)
    {
        _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:TokenKey"]!));
        _configuration = configuration;
    }

    public string CreateAccessToken(User user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var credentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(int.Parse(_configuration["JWT:ExpiryInDays"]!)),
            SigningCredentials = credentials,
            Issuer = _configuration["JWT:Issuer"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
