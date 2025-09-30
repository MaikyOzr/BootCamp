using BootCamp.Application.Feature.Auth.Models.Request;
using BootCamp.Application.Feature.Auth.SingIn.Command;
using BootCamp.Application.Feature.ValidationService;
using FluentValidation;
using Wk1.Middlewere;
using Wk1.ProblemDetails;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddOpenApi();
builder.Services.AddScoped<SignInCommand>();
builder.Services.AddScoped<IValidator<SingInRequst>, SignInRequestValidation>();
builder.Services.AddValidation();
builder.Services.AddLogging();
builder.Services.AddHttpClient();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();

app.MapPost("/user", async 
    (HttpContext context, SingInRequst requst, SignInCommand command,
    IValidator<SingInRequst> validator, CancellationToken ct) =>
{

    var validationResult = validator.Validate(requst);
    if (!validationResult.IsValid)
    {
        var errors = validationResult.Errors.Select(e => e.ErrorMessage).FirstOrDefault();
        var pd = new ProblemDetail
        {
            Type = "https://example.com/probs/internal-server-error",
            Title = "Validation",
            Status = 400,
            Detail = errors,
            Instance = context.Request.Path
        };
        return Results.BadRequest(new { Error = pd.Detail, Code = pd.Status });
    }
    var res = await command.ExecuteAsync(requst, ct);

    return Results.Ok(res);
});

app.Run();

