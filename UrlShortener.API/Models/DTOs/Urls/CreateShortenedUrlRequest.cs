namespace UrlShortener.API.Models.DTOs.Urls;

public class CreateShortenedUrlRequest
{
    public string Url { get; set; } = string.Empty;
}
