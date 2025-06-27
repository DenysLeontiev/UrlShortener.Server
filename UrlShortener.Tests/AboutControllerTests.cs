using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Security.Claims;
using UrlShortener.API.Controllers;
using UrlShortener.API.Models.Entities;
using UrlShortener.API.Models;
using UrlShortener.API.Interfaces.Persistence;

public class AboutControllerTests
{
    private readonly Mock<IAboutPageRepository> _repoMock;
    private readonly AboutController _controller;

    public AboutControllerTests()
    {
        _repoMock = new Mock<IAboutPageRepository>();
        _controller = new AboutController(_repoMock.Object);
    }

    [Fact]
    public async Task Get_ReturnsOk_WithAboutPage()
    {
        var aboutPage = new AboutPage
        {
            Id = "1",
            Content = "Test content",
            EditedById = "admin-id"
        };

        _repoMock.Setup(r => r.Get()).ReturnsAsync(aboutPage);

        var result = await _controller.Get();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<AboutPage>(ok.Value);
        Assert.Equal("Test content", returned.Content);
    }

    [Fact]
    public async Task Edit_ReturnsOk_WithUpdatedPage()
    {
        var request = new EditAboutPageRequest { NewContent = "Updated content" };
        var userId = "admin-id";
        var updatedPage = new AboutPage
        {
            Id = "1",
            Content = "Updated content",
            EditedById = userId
        };

        _repoMock.Setup(r => r.Edit(request.NewContent, userId)).ReturnsAsync(updatedPage);

        var httpContext = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }))
        };
        _controller.ControllerContext.HttpContext = httpContext;

        var result = await _controller.Edit(request);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<AboutPage>(ok.Value);
        Assert.Equal("Updated content", returned.Content);
        Assert.Equal(userId, returned.EditedById);
    }

    [Fact]
    public async Task Edit_ReturnsUnauthorized_IfUserIdIsNull()
    {
        var request = new EditAboutPageRequest { NewContent = "Invalid try" };
        _controller.ControllerContext.HttpContext = new DefaultHttpContext(); // no user

        var result = await _controller.Edit(request);

        Assert.IsType<UnauthorizedResult>(result.Result);
    }
}
