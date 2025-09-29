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


app.MapGet("/user", async (IHttpContextAccessor httpContext, CancellationToken ct) => {
    var users = await GetUsersFromDbAsync(httpContext, ct);
    return users;
});

app.Run();

async Task<List<string>> GetUsersFromDbAsync(IHttpContextAccessor httpContext, CancellationToken ct)
{
    await Task.Delay(1000, ct); // імітація доступу до БД
    return new List<string> { "Alice", "Bob", "Charlie" };
}

