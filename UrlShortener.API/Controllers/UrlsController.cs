using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.API.ExtensionMethods;
using UrlShortener.API.Interfaces.Persistence;
using UrlShortener.API.Models.DTOs.Urls;
using UrlShortener.API.Pagination;

namespace UrlShortener.API.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UrlsController : BaseApiController
{
    private readonly IUrlRepository _urlRepository;
    private readonly IMapper _mapper;

    public UrlsController(IUrlRepository urlRepository,
        IMapper mapper)
    {
        _urlRepository = urlRepository;
        _mapper = mapper;
    }

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UrlDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<UrlDto>>> Get([FromRoute] UrlParams urlParams)
    {
        var paginatedUrls = await _urlRepository.GetUrls(urlParams);

        Response.AddPaginationHeaders(new PaginationHeader(
            paginatedUrls.CurrentPage,
            paginatedUrls.PageSize,
            paginatedUrls.TotalCount,
            paginatedUrls.TotalPages));

        var mappedUrls = _mapper.Map<IEnumerable<UrlDto>>(paginatedUrls);

        return Ok(mappedUrls);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UrlDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UrlDto>> GetById(string id)
    {
        var shortenedUrl = await _urlRepository.GetById(id);

        var mappedShortenedUrl = _mapper.Map<UrlDto>(shortenedUrl);

        return Ok(mappedShortenedUrl);
    }

    [HttpPost("shorten")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UrlDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UrlDto>> Create([FromBody] CreateShortenedUrlRequest shortenedUrlRequest)
    {
        if (!Uri.TryCreate(shortenedUrlRequest.Url, UriKind.Absolute, out _))
        {
            return BadRequest("Wrong url argument");
        }

        string currentUserId = User.GetUserId()!;
        var shortenedUrl = await _urlRepository.CreateShortenUrl(shortenedUrlRequest.Url, currentUserId);

        var mappedShortenedUrl = _mapper.Map<UrlDto>(shortenedUrl);

        return CreatedAtAction(nameof(GetById), new { id = mappedShortenedUrl.Id }, mappedShortenedUrl);
    }
}
