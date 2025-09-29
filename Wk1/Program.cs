using FluentValidation;
using Wk1;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IValidator<UserCreateDTO>, ValidationUser>();
builder.Services.AddValidation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();

app.MapGet("/user", async (IHttpContextAccessor httpContext, CancellationToken ct) => {
    try
    {
        var users = await GetUsersFromDbAsync(ct);
        return Results.Ok(users);
    }
    catch (OperationCanceledException ex)
    {
        httpContext.HttpContext?.Response.Headers.Add("X-Error-Message", "Request was cancelled by the client.");
        return Results.StatusCode(499);
    }
});

app.MapPost("/user", async (HttpContext context, UserCreateDTO user, IValidator<UserCreateDTO> validator, CancellationToken ct) =>
{
    var validationResult = validator.Validate(user);
    if (!validationResult.IsValid)
    {
        var errors = validationResult.Errors.Select(e => e.ErrorMessage).FirstOrDefault();
        var pd = new ProblemDetails
        {
            Type = "https://example.com/probs/internal-server-error",
            Title = "Validation",
            Status = 400,
            Detail = errors,
            Instance = context.Request.Path
        };
        return Results.BadRequest(new { Error = pd.Detail, Code = pd.Status});
    }
    // Логіка створення користувача
    return Results.Created($"/user/{Guid.NewGuid()}", user);
});

app.Run();

async Task<List<string>> GetUsersFromDbAsync(CancellationToken ct)
{
    ct.ThrowIfCancellationRequested();
    await Task.Delay(5000, ct); // імітація доступу до БД
    return new List<string> { "Alice", "Bob", "Charlie" };
}

internal class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var pd = new ProblemDetails
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

internal class CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
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