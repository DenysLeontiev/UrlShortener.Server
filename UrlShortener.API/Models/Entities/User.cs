using Microsoft.AspNetCore.Identity;

namespace UrlShortener.API.Models.Entities;

public class User : IdentityUser
{
    public List<Url> Urls { get; set; } = new ();
}
