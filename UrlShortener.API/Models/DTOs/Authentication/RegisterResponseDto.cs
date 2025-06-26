namespace UrlShortener.API.Models.DTOs.Authentication;

public class RegisterResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}
