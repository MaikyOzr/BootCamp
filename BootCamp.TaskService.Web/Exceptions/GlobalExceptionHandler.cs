using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BootCamp.TaskService.Web.Exceptions;

internal sealed class GlobalExceptionHandler
    (IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{

    async ValueTask<bool> IExceptionHandler.TryHandleAsync
        (HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = exception switch
        {
            ApplicationException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError,
        };

        logger.LogError(exception, "An unhandled exception has occurred while executing the request.");

        await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Type = exception.GetType().Name,
                Title = "An error occured",
                Detail = exception.Message,
                Status = StatusCodes.Status500InternalServerError,
            }
        });
        return true;
    }
}
