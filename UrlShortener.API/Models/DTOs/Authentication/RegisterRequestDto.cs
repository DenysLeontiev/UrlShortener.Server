﻿namespace UrlShortener.API.Models.DTOs.Authentication;

public class RegisterRequestDto
{
    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
