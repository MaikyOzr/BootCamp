using BootCamp.UserService;
using BootCamp.UserService.Web;
using BootCamp.UserService.Web.Middlewere;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));


builder.Services.AddServices(builder.Configuration);

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

app.MapAuthEndpoints();

app.UseMiddleware<CustomRequestLogMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.Run();

