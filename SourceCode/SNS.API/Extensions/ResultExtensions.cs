using Microsoft.AspNetCore.Mvc;
using SNS.Common.Results;

namespace SNS.API.Extensions;

public static class ResultExtensions
{
    public static ActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        if (result.StatusCode.Code == 200)
        {
            return controller.Ok(result);
        }

        return MapStatus(result, controller);
    }

    public static ActionResult ToActionResult(this Result result, ControllerBase controller)
    {
        if (result.StatusCode.Code == 200)
        {
            return controller.Ok(new { message = "Success" });
        }

        return MapStatus(result, controller);
    }

    private static ActionResult MapStatus(Result result, ControllerBase controller)
    {
        return result.StatusCode.Code switch
        {
            400 => controller.BadRequest(result),
            401 => controller.Unauthorized(result),
            403 => controller.StatusCode(403, result), // Forbidden
            404 => controller.NotFound(result),
            409 => controller.Conflict(result),
            422 => controller.UnprocessableEntity(result),

            500 => controller.StatusCode(500, result),

            _ => controller.StatusCode(result.StatusCode.Code, result)
        };
    }
}