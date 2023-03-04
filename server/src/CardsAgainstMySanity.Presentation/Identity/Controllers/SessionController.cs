namespace CardsAgainstMySanity.Presentation.Identity.Controllers;

using CardsAgainstMySanity.Domain.Identity.Requests;
using CardsAgainstMySanity.Domain.Identity.Tokens;
using CardsAgainstMySanity.Presentation.Identity.ViewModels;
using CardsAgainstMySanity.Presentation.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[AllowAnonymous]
[Route("api/sessions")]
public class SessionController : ControllerBase
{
    private readonly IMediator _mediator;

    public SessionController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [Route("")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GuestViewModel))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationErrorResponse))]
    public async Task<IActionResult> Store([FromBody] CreateGuestSessionRequest request)
    {
        var result = await _mediator.Send(request);

        if (result.Failed)
            return BadRequest(new ValidationErrorResponse(result.Error));

        var (guest, accessToken, refreshToken) = result.Data;

        SetTokenCookie("cams_accesstoken", accessToken);
        SetTokenCookie("cams_refreshtoken", refreshToken);

        return Created(
            Url.Link("GetGuestById", new { guest.Id }),
            GuestViewModel.FromEntity(guest));
    }

    [HttpPost]
    [Route("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationErrorResponse))]
    public async Task<IActionResult> Refresh()
    {
        var request = new RefreshTokenRequest(Request.Cookies["cams_refreshtoken"]);
        var result = await _mediator.Send(request);

        if (result.Failed)
            return BadRequest();

        var (accessToken, refreshToken) = result.Data;

        SetTokenCookie("cams_accesstoken", accessToken);
        SetTokenCookie("cams_refreshtoken", refreshToken);

        return NoContent();
    }

    private void SetTokenCookie(string name, Token token)
        => Response.Cookies.Append(name, token.TokenValue, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Secure = true,
            Expires = token.ExpiresAt
        });
}