namespace CardsAgainstMySanity.Presentation.Auth.Controllers;

using System.Diagnostics;
using CardsAgainstMySanity.Domain.Auth.Dtos;
using CardsAgainstMySanity.Domain.Auth.Services;
using CardsAgainstMySanity.Presentation.Auth.Middlewares;
using CardsAgainstMySanity.Presentation.Shared;
using Microsoft.AspNetCore.Mvc;

[Route("api/guests")]
[ApiController]
public class GuestController : BaseApiController
{
    private readonly GuestService _guestService;

    public GuestController(GuestService guestService) => this._guestService = guestService;

    [Route("")]
    [HttpPost]
    public async Task<IActionResult> InitSession(GuestInitSessionDto dto)
    {
        try
        {
            dto.IpAddress = this.GetIpAddress();
            var result = await this._guestService.InitSession(dto);

            if (result.Failed)
            {
                return this.BadRequest("Failed to init session");
            }

            var guest = result.Data;
            this.Response.Cookies.Append("cards_accesstoken", guest.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict
            });
            var refreshToken = guest.RefreshTokens.OrderBy(t => t.CreatedAt).Last().Token.ToString();
            this.Response.Cookies.Append("cards_refreshtoken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict
            });

            return this.Created(this.Url.Link("ShowGuest", new { guest.Id }), GuestLoginViewModel.FromGuest(guest));
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            return this.StatusCode(500);
        }
    }

    [HttpGet]
    [Route("{id:guid}", Name = "ShowGuest")]
    [ValidateTokens]
    public async Task<IActionResult> Show([FromRoute] Guid id)
    {
        try
        {
            var guest = await this._guestService.GetGuestById(id);
            if (guest == null)
            {
                return this.NotFound();
            }

            return this.Ok(GuestViewModel.FromGuest(guest));
        }
        catch (Exception e)
        {
            Debug.WriteLine(e.Message);
            return this.InternalServerError();
        }
    }
}
