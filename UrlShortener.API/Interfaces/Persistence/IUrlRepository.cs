using UrlShortener.API.Models.Entities;
using UrlShortener.API.Pagination;

namespace UrlShortener.API.Interfaces.Persistence;

public interface IUrlRepository
{
    Task<PagedList<Url>> GetUrlsAsync(UrlParams urlParams);
    Task<Url> GetByIdAsync(string id);
    Task<Url> GetOriginalUrlAsync(string code);
    Task<Url> CreateShortenUrlAsync(string url, string currentUserId);
    Task DeleteShortenedUrlByIdAsync(string id);
}
