namespace UrlShortener.API.Models.DTOs.Urls;

public class UrlDto
{
    public string Id { get; set; } = string.Empty;

    public string OriginalUrl { get; set; } = string.Empty;
    public string ShortenUrl { get; set; } = string.Empty;

    public DateTime DateModified { get; set; }
    public DateTime DateCreated { get; set; }

    public string UserName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}
