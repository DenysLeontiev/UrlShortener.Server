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

    public async Task<PagedList<Url>> GetUrlsAsync(UrlParams urlParams)
    {
        var urlsQuery = _dbContext.Urls
            .Include(x => x.User)
            .OrderByDescending(x => x.DateCreated)
            .AsNoTracking();

        return await PagedList<Url>.CreateAsync(urlsQuery, urlParams.PageNumber, urlParams.PageSize);
    }

    public async Task<Url> GetByIdAsync(string id)
    {
        var url = await _dbContext.Urls
            .Include(x => x.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id!.Equals(id));

        return url!;
    }

    public async Task<Url> GetOriginalUrlAsync(string code)
    {
        var url = await _dbContext.Urls
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code!.Equals(code));

        return url!;
    }

    public async Task<Url> CreateShortenUrlAsync(string url, string currentUserId)
    {
        var code = await _urlShorteningService.GenerateUniqueCodeAsync();

        var shortenedUrl = new Url
        {
            Id = Guid.NewGuid().ToString(),
            OriginalUrl = url,
            Code = code,
            ShortenUrl = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext!.Request.Host}/{code}",
            UserId = currentUserId,
        };

        await _dbContext.Urls.AddAsync(shortenedUrl);

        await _dbContext.SaveChangesAsync();

        return shortenedUrl;
    }

    public async Task DeleteShortenedUrlByIdAsync(string id)
    {
        await _dbContext.Urls.Where(x => x.Id.Equals(id)).ExecuteDeleteAsync();
    }

    public async Task DeleteAllAsync()
    {
        await _dbContext.Urls.ExecuteDeleteAsync();
    }
}
