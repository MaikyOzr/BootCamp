namespace Wk1.Middlewere;

public class CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault();
        if (string.IsNullOrEmpty(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
            context.Request.Headers.Add("X-Correlation-ID", correlationId);
        }
        context.Response.Headers.Add("X-Correlation-ID", correlationId);
        logger.LogInformation("Correlation ID: {CorrelationId}", correlationId);
        await next(context);
    }
}
