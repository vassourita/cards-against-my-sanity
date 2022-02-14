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
            var result = await accessService.ValidateUserTokens(accessToken, refreshToken);

            if (result.Failed)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }
}