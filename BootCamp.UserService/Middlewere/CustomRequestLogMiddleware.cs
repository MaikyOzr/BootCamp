using Serilog.Context;

namespace BootCamp.UserService.Web.Middlewere;

internal sealed class CustomRequestLogMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(HttpContext context)
    {
        using (LogContext.PushProperty("CorrelationId", context.TraceIdentifier)) 
        {
            return next(context);
        }
    }
}
