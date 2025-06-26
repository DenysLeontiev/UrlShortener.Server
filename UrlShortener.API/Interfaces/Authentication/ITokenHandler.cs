using UrlShortener.API.Models.Entities;

namespace UrlShortener.API.Interfaces.Authentication;

public interface ITokenHandler
{
    string CreateAccessToken(User user, IEnumerable<string> roles);
}
