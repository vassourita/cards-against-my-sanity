using CardsAgainstMySanity.Domain.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CardsAgainstMySanity.Presentation.Auth.Middlewares
{
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class ValidateTokensAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var accessToken = context.HttpContext.Request.Cookies["cards_accesstoken"];
            var refreshToken = context.HttpContext.Request.Cookies["cards_refreshtoken"];

            var accessService = context.HttpContext.RequestServices.GetRequiredService<AccessService>();

            var result = await accessService.ValidateUserTokens(accessToken, Guid.Parse(refreshToken));

            if (result.Failed)
            {
                context.HttpContext.Response.Cookies.Delete("cards_accesstoken");
                context.HttpContext.Response.Cookies.Delete("cards_refreshtoken");
                context.HttpContext.Response.Headers.Add("Cards-Tokens", result.Error.ToString());
                context.Result = new UnauthorizedResult();
                return;
            }

            var (principal, newAccessToken) = result.Data;
            context.HttpContext.Response.Cookies.Append("cards_accesstoken", newAccessToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict
            });
            context.HttpContext.User = principal;
        }
    }
}