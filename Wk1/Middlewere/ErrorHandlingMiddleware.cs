using BootCamp.Application;
using Microsoft.EntityFrameworkCore;

namespace Wk1.Middlewere;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var pd = new ProblemDetail
        {
            Type = "https://example.com/probs/internal-server-error",
            Title = "Internal Server Error",
            Status = 500,
            Detail = "An unexpected error occurred on the server.",
            Instance = context.Request.Path
        };

        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception has occurred while executing the request.");
            await context.Response.WriteAsJsonAsync(new { pd.Detail, pd.Status });
        }
    }
}
