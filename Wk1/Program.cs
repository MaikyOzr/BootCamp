using Serilog;
using Wk1;
using Wk1.Endpoints;
using Wk1.Middlewere;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddWk1Services(builder.Configuration);
var app = builder.Build();
var env = builder.Environment;

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    builder.Configuration.AddUserSecrets<Program>(optional: true);
}

app.UseAuthentication();
app.UseAuthorization();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapTaskEndpoints();
app.MapTaskCommentEndpoints();
app.MapAuthEndpoints();

app.UseMiddleware<CustomRequestLogMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.Run();