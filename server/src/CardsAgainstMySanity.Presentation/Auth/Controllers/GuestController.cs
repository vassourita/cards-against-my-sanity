using System.Diagnostics;
using CardsAgainstMySanity.Domain.Auth.Dtos;
using CardsAgainstMySanity.Domain.Auth.Services;
using CardsAgainstMySanity.Presentation.Shared;
using Microsoft.AspNetCore.Mvc;

namespace CardsAgainstMySanity.Presentation.Auth.Controllers
{
    [Route("api/guests")]
    [ApiController]
    public class GuestController : BaseApiController
    {
        private readonly GuestService _guestService;

        public GuestController(GuestService guestService)
        {
            _guestService = guestService;
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> InitSession(GuestInitSessionDto dto)
        {
            if (IsInvalidModelState(out var response))
                return response;

            try
            {
                var ipAddress = GetIpAddress();
                var result = await _guestService.InitSession(dto, ipAddress);

                if (result.Failed)
                {
                    return BadRequest("Failed to init session");
                }

                var guest = result.Data;
                Response.Cookies.Append("cards_accesstoken", guest.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict
                });
                var refreshToken = guest.RefreshTokens.OrderBy(t => t.CreatedAt).Last().Token.ToString();
                Response.Cookies.Append("cards_refreshtoken", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict
                });

                return Created(Url.Link("ShowGuest", new { Id = guest.Id }), GuestLoginViewModel.FromGuest(guest));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("{id:guid}", Name = "ShowGuest")]
        public async Task<IActionResult> Show([FromRoute] Guid id)
        {
            try
            {
                var guest = await _guestService.GetGuestById(id);
                if (guest == null)
                {
                    return NotFound();
                }

                return Ok(GuestViewModel.FromGuest(guest));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return InternalServerError();
            }
        }
    }
}