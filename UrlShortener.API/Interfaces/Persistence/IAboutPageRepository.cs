using UrlShortener.API.Models.Entities;

namespace UrlShortener.API.Interfaces.Persistence;

public interface IAboutPageRepository
{
    Task<AboutPage> Get();
    Task<AboutPage> Edit(string newContent, string userId);
}
