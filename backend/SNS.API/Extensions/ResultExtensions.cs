using Microsoft.AspNetCore.Mvc;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

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
            return controller.Ok(result);
        }

        return MapStatus(result, controller);
    }

    private static ActionResult MapStatus(Result result, ControllerBase controller)
    {
        var code = GetHttpStatusCode(result.StatusCode);

        return code switch
        {
            200 => controller.Ok(result),
            201 => controller.Created("", result),
            202 => controller.Accepted(result),
            204 => controller.NoContent(),
            400 => controller.BadRequest(result),
            401 => controller.Unauthorized(result),
            403 => controller.StatusCode(403, result), // Forbidden
            404 => controller.NotFound(result), 
            405 => controller.StatusCode(405, result), // Method Not allowed 
            406 => controller.StatusCode(406, result), // Not Acceptable
            409 => controller.Conflict(result),
            410 => controller.StatusCode(410, result),
            411 => controller.StatusCode(411, result), // Length Required
            422 => controller.UnprocessableEntity(result),
            429 => controller.StatusCode(429, result), // Too Many Requests

            500 => controller.StatusCode(500, result),

            _ => controller.StatusCode(result.StatusCode.Code, result)
        };
    }

    private static int GetHttpStatusCode(StatusCode statusCode)
    {
        var code = statusCode.Code;

        while (code > 999)
        {
            code /= 10;
        }

        return code;
    }
}
