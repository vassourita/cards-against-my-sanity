using System.Diagnostics;
using CardsAgainstMySanity.Domain.Auth.Dtos;
using CardsAgainstMySanity.Domain.Auth.Services;
using Microsoft.AspNetCore.Mvc;

namespace CardsAgainstMySanity.Presentation.Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuestController : ControllerBase
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
            try
            {
                dto.IpAddress = HttpContext.Connection.RemoteIpAddress.ToString();
                var result = await _guestService.InitSession(dto);

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
                return Ok(GuestLoginViewModel.FromGuest(guest));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return StatusCode(500);
            }
        }
    }
}