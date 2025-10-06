using Microsoft.AspNetCore.Mvc;

namespace Wk1.Middlewere;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {

            context.Response.StatusCode = ex switch
            {
                ApplicationException => StatusCodes.Status400BadRequest, 
                _ => StatusCodes.Status500InternalServerError,
            };

            logger.LogError(ex, "An unhandled exception has occurred while executing the request.");
            throw ex;
        }
    }
}
