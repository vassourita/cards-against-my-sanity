namespace CardsAgainstMySanity.Presentation.Shared;

using Microsoft.AspNetCore.Mvc;

public class BaseApiController : ControllerBase
{
    protected StatusCodeResult InternalServerError() => this.StatusCode(500);

    protected bool IsInvalidModelState(out BadRequestObjectResult response)
    {
        if (!this.ModelState.IsValid)
        {
            response = this.BadRequest(this.ModelState.SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage));
            return true;
        }
        response = null;
        return false;
    }

    protected string GetIpAddress() => this.Request.HttpContext.Connection.RemoteIpAddress.ToString();
}
