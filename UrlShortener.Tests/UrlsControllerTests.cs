using Xunit;
using Moq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using UrlShortener.API.Controllers;
using UrlShortener.API.Interfaces.Persistence;
using UrlShortener.API.Models.DTOs.Urls;
using UrlShortener.API.Models.Entities;
using UrlShortener.API.Pagination;

public class UrlsControllerTests
{
    private readonly Mock<IUrlRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UrlsController _controller;

    public UrlsControllerTests()
    {
        _repoMock = new Mock<IUrlRepository>();
        _mapperMock = new Mock<IMapper>();
        _controller = new UrlsController(_repoMock.Object, _mapperMock.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user-id")
        }));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task Get_ReturnsOkWithDtosAndPagination()
    {
        var urlParams = new UrlParams { PageNumber = 1, PageSize = 10 };
        var urls = new List<Url>
        {
            new Url
            {
                Id = "1",
                OriginalUrl = "https://x.com",
                ShortenUrl = "short.ly/1",
                UserId = "user-id"
            }
        };

        var pagedList = new PagedList<Url>(urls, urls.Count, 1, 10);

        var dtos = new List<UrlDto>
        {
            new UrlDto
            {
                Id = "1",
                OriginalUrl = "https://x.com",
                ShortenUrl = "short.ly/1",
                UserId = "user-id",
                UserName = "TestUser",
                DateCreated = System.DateTime.UtcNow,
                DateModified = System.DateTime.UtcNow
            }
        };

        _repoMock.Setup(r => r.GetUrlsAsync(urlParams)).ReturnsAsync(pagedList);
        _mapperMock.Setup(m => m.Map<IEnumerable<UrlDto>>(pagedList)).Returns(dtos);

        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext.HttpContext = httpContext;

        var result = await _controller.Get(urlParams);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var returnedDtos = Assert.IsAssignableFrom<IEnumerable<UrlDto>>(ok.Value);
        Assert.Single(returnedDtos);
        Assert.Equal("https://x.com", returnedDtos.First().OriginalUrl);
        Assert.True(httpContext.Response.Headers.ContainsKey("Pagination"));
    }

    [Fact]
    public async Task GetById_ReturnsOk()
    {
        var id = "123";
        var url = new Url
        {
            Id = id,
            OriginalUrl = "https://google.com",
            ShortenUrl = "short.ly/g",
            UserId = "user-id"
        };

        var dto = new UrlDto
        {
            Id = id,
            OriginalUrl = url.OriginalUrl,
            ShortenUrl = url.ShortenUrl,
            UserId = url.UserId,
            UserName = "TestUser",
            DateCreated = System.DateTime.UtcNow,
            DateModified = System.DateTime.UtcNow
        };

        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(url);
        _mapperMock.Setup(m => m.Map<UrlDto>(url)).Returns(dto);

        var result = await _controller.GetById(id);
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(dto, ok.Value);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenUrlInvalid()
    {
        var request = new CreateShortenedUrlRequest { Url = "not-a-url" };
        var result = await _controller.Create(request);
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAt_WhenValid()
    {
        var request = new CreateShortenedUrlRequest { Url = "https://example.com" };
        var url = new Url
        {
            Id = "abc",
            OriginalUrl = request.Url,
            ShortenUrl = "short.ly/abc",
            UserId = "user-id"
        };

        var dto = new UrlDto
        {
            Id = "abc",
            OriginalUrl = request.Url,
            ShortenUrl = url.ShortenUrl,
            UserId = "user-id",
            UserName = "TestUser",
            DateCreated = System.DateTime.UtcNow,
            DateModified = System.DateTime.UtcNow
        };

        _repoMock.Setup(r => r.CreateShortenUrlAsync(request.Url, "user-id")).ReturnsAsync(url);
        _mapperMock.Setup(m => m.Map<UrlDto>(url)).Returns(dto);

        var result = await _controller.Create(request);
        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(dto, created.Value);
        Assert.Equal("GetById", created.ActionName);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_IfNotOwner()
    {
        var url = new Url { Id = "x", OriginalUrl = "https://a.com", UserId = "another-user" };
        _repoMock.Setup(r => r.GetByIdAsync("x")).ReturnsAsync(url);

        var result = await _controller.Delete("x");
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsOk_IfOwner()
    {
        var url = new Url { Id = "x", OriginalUrl = "https://a.com", UserId = "user-id" };
        _repoMock.Setup(r => r.GetByIdAsync("x")).ReturnsAsync(url);

        var result = await _controller.Delete("x");
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteAll_ReturnsNoContent()
    {
        var result = await _controller.DeleteAll();
        _repoMock.Verify(r => r.DeleteAllAsync(), Times.Once);
        Assert.IsType<NoContentResult>(result);
    }
}
