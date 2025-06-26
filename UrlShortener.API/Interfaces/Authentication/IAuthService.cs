using UrlShortener.API.Models.DTOs.Authentication;

namespace UrlShortener.API.Interfaces.Authentication;

public interface IAuthService
{
    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
}
