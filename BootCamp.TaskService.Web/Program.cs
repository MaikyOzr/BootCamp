using BootCamp.TaskService.Web;
using BootCamp.TaskService.Web.Endpoints;
using BootCamp.TaskService.Web.Middlewere;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));


builder.Services.AddTaskServices(builder.Configuration);

var app = builder.Build();
var env = builder.Environment;

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    builder.Configuration.AddUserSecrets<Program>(optional: true);
}

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.MapTaskEndpoints();

app.UseMiddleware<CustomRequestLogMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.Run();



