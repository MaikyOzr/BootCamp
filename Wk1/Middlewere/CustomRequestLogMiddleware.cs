using Serilog.Context;

namespace Wk1.Middlewere;

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
