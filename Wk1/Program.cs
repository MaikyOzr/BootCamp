using Wk1;
using Wk1.Endpoints;
using Wk1.Middlewere;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddWk1Services(builder.Configuration);
var app = builder.Build();
var env = builder.Environment;

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    builder.Configuration.AddUserSecrets<Program>(optional: true);
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.MapTaskEndpoints();
app.MapTaskCommentEndpoints();
app.MapAuthEndpoints();
app.UseExceptionHandler();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.Run();