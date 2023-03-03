namespace CardsAgainstMySanity.Presentation.Shared;

using Microsoft.AspNetCore.Mvc;

public class BaseApiController : ControllerBase
{
    protected StatusCodeResult InternalServerError() => StatusCode(500);

    protected bool IsInvalidModelState(out BadRequestObjectResult response)
    {
        if (!ModelState.IsValid)
        {
            response = BadRequest(ModelState.SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage));
            return true;
        }
        response = null;
        return false;
    }

    protected string GetIpAddress() => Request.HttpContext.Connection.RemoteIpAddress.ToString();
}
