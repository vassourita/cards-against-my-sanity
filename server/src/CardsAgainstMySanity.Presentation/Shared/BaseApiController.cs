using Microsoft.AspNetCore.Mvc;

namespace CardsAgainstMySanity.Presentation.Shared
{
    public class BaseApiController : ControllerBase
    {
        protected StatusCodeResult InternalServerError()
        {
            return StatusCode(500);
        }

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

        protected string GetIpAddress()
        {
            return Request.HttpContext.Connection.RemoteIpAddress.ToString();
        }
    }
}