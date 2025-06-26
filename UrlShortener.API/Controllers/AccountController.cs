using Microsoft.AspNetCore.Mvc;
using UrlShortener.API.Interfaces.Authentication;
using UrlShortener.API.Models.DTOs.Authentication;

namespace UrlShortener.API.Controllers;

public class AccountController : BaseApiController
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RegisterResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RegisterResponseDto>> Register(RegisterRequestDto request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);

            return Created("", response);
        }
        catch (ArgumentNullException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);

            return Ok(response);
        }
        catch (ArgumentNullException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
