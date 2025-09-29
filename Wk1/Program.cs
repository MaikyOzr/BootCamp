using Wk1;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapGet("/user", async (IHttpContextAccessor httpContext, CancellationToken ct) => {
    try
    {
        var users = await GetUsersFromDbAsync(ct);
        return Results.Ok(users);
    }
    catch (OperationCanceledException ex)
    {
        httpContext.HttpContext?.Response.Headers.Add("X-Error-Message", "Request was cancelled by the client.");
        return Results.StatusCode(499); // 499 Client Closed Request (неофіційний, але популярний)
    }
});

app.Run();

async Task<List<string>> GetUsersFromDbAsync(CancellationToken ct)
{
    ct.ThrowIfCancellationRequested();
    await Task.Delay(5000, ct); // імітація доступу до БД
    return new List<string> { "Alice", "Bob", "Charlie" };
}

internal class ErrorHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ErrorHandlingMiddleware> logger;
    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }
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
            await context.Response.WriteAsJsonAsync(new { error = pd.Detail });
        }
    }
}