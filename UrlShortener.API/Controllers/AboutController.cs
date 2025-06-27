using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using UrlShortener.API.ExtensionMethods;
using UrlShortener.API.Interfaces.Persistence;
using UrlShortener.API.Models;
using UrlShortener.API.Models.Consts;
using UrlShortener.API.Models.Entities;

namespace UrlShortener.API.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AboutController : BaseApiController
{
    private readonly IAboutPageRepository _aboutPageRepository;

    public AboutController(IAboutPageRepository aboutPageRepository)
    {
        _aboutPageRepository = aboutPageRepository;
    }

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(AboutPage), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<AboutPage>> Get()
    {
        var aboutPage = await _aboutPageRepository.Get();
        return Ok(aboutPage);
    }

    [HttpPost("edit")]
    [Authorize(Roles = DbRolesConsts.AdminRole)]
    [ProducesResponseType(typeof(AboutPage), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<AboutPage>> Edit([FromBody] EditAboutPageRequest request)
    {
        var userId = User.GetUserId()!;

        if (userId is null)
            return Unauthorized();

        var aboutPage = await _aboutPageRepository.Edit(request.NewContent, userId);
        return Ok(aboutPage);
    }
}
