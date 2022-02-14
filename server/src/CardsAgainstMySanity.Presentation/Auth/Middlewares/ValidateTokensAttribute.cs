using CardsAgainstMySanity.Domain.Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CardsAgainstMySanity.Presentation.Auth.Middlewares
{
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class ValidateTokensAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var accessToken = context.HttpContext.Request.Cookies["cards_accesstoken"];
            var refreshToken = context.HttpContext.Request.Cookies["cards_refreshtoken"];

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                context.Result = new UnauthorizedResult();
                return Task.CompletedTask;
            }

            var accessService = context.HttpContext.RequestServices.GetRequiredService<AccessService>();
            if (accessService.IsAccessTokenValid(accessToken, out var principal))
            {
                context.HttpContext.User = principal; //principal claims contains name and id
            }
            else
            {
                context.Result = new UnauthorizedResult();
            }

            return Task.CompletedTask;
        }
    }
}