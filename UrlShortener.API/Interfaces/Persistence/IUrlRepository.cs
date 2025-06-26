using UrlShortener.API.Models.Entities;
using UrlShortener.API.Pagination;

namespace UrlShortener.API.Interfaces.Persistence;

public interface IUrlRepository
{
    Task<PagedList<Url>> GetUrls(UrlParams urlParams);
    Task<Url> GetById(string id);
    Task<Url> CreateShortenUrl(string url, string currentUserId);
}
