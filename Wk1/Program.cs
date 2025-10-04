using Testcontainers.PostgreSql;
using Wk1;
using Wk1.Endpoints;
using Wk1.Middlewere;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWk1Services(builder.Configuration);

var postgreSqlContainer = new PostgreSqlBuilder().Build();
await postgreSqlContainer.StartAsync();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();
app.MapTaskEndpoints();
app.MapTaskCommentEndpoints();
app.MapAuthEndpoints();

app.Run();