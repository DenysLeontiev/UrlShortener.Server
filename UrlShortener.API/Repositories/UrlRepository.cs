using Microsoft.EntityFrameworkCore;
using UrlShortener.API.Data;
using UrlShortener.API.Interfaces.Persistence;
using UrlShortener.API.Interfaces.Urls;
using UrlShortener.API.Models.Entities;
using UrlShortener.API.Pagination;

namespace UrlShortener.API.Repositories;

public class UrlRepository : IUrlRepository
{
    private readonly UrlShortenerDbContext _dbContext;
    private readonly UrlShorteningService _urlShorteningService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UrlRepository(UrlShortenerDbContext dbContext,
        UrlShorteningService urlShorteningService,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _urlShorteningService = urlShorteningService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PagedList<Url>> GetUrls(UrlParams urlParams)
    {
        var urlsQuery = _dbContext.Urls
            .Include(x => x.User)
            .OrderByDescending(x => x.DateCreated)
            .AsNoTracking();

        return await PagedList<Url>.CreateAsync(urlsQuery, urlParams.PageNumber, urlParams.PageSize);
    }

    public async Task<Url> GetById(string id)
    {
        var url = await _dbContext.Urls
            .Include(x => x.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id!.Equals(id));

        return url!;
    }


    public async Task<Url> CreateShortenUrl(string url, string currentUserId)
    {
        var code = await _urlShorteningService.GenerateUniqueCode();

        var shortenedUrl = new Url
        {
            Id = Guid.NewGuid().ToString(),
            OriginalUrl = url,
            Code = code,
            ShortenUrl = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://${_httpContextAccessor.HttpContext!.Request.Host}/api/{code}",
            UserId = currentUserId,
        };

        await _dbContext.Urls.AddAsync(shortenedUrl);

        await _dbContext.SaveChangesAsync();

        return shortenedUrl;
    }
}
