using BootCamp.Application.Feature.Auth.Models.Request;
using BootCamp.Application.Feature.Auth.SingIn.Command;
using BootCamp.Application.Feature.Task.Command;
using BootCamp.Application.Feature.Task.Models.Request;
using BootCamp.Application.Feature.Task.Query;
using BootCamp.Application.Feature.TaskCommentFeature.Command;
using BootCamp.Application.Feature.TaskCommentFeature.Models.Request;
using BootCamp.Application.Feature.TaskCommentFeature.Query;
using BootCamp.Application.Feature.ValidationService;
using BootCamp.Application.ValidationService;
using BootCamp.Infrastruture;
using FluentValidation;
using Wk1.Middlewere;
using Wk1.ProblemDetails;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddOpenApi();
builder.Services.AddScoped<SignInCommand>();
builder.Services.AddScoped<CreateTaskCommand>();
builder.Services.AddScoped<GetByIdTaskQuery>();
builder.Services.AddScoped<GetAllTaskQuery>();
builder.Services.AddScoped<GetByIdTaskCommentQuery>();
builder.Services.AddScoped<CreateTaskCommentCommand>();
builder.Services.AddScoped<IValidator<CreateTaskCommentRequest>, CreateTaskCommentRequestValidation>();
builder.Services.AddScoped<IValidator<CreateTaskRequest>, CreateTaskRequestValidation>();
builder.Services.AddScoped<IValidator<SingInRequst>, SignInRequestValidation>();
builder.Services.AddValidation();
builder.Services.AddLogging();
builder.Services.AddHttpClient();
builder.Services.AddDbContext<AppDbContext>();
builder.Services
    .AddNpgsql<AppDbContext>(builder.Configuration.GetConnectionString("DefaultConnection"));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();

app.MapPost("/user", async 
    (HttpContext context, SingInRequst request, SignInCommand command,
    IValidator<SingInRequst> validator, CancellationToken ct) =>
{
    var validationResult = validator.Validate(request);
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
    var res = await command.ExecuteAsync(request, ct);

    return Results.Ok(res);
});

app.MapPost("/task", async (HttpContext context, CreateTaskRequest request,
    CreateTaskCommand command, IValidator<CreateTaskRequest> validator,
    CancellationToken ct) => 
{
    var validationResult = validator.Validate(request);
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
    var res = await command.ExecuteAsync(request, ct);

    return Results.Ok(res);
});

app.MapGet("/task/{id:guid}", async 
    (Guid id, GetByIdTaskQuery query, CancellationToken ct) =>
{
    var res = await query.ExecuteAsync(id, ct);
    return Results.Ok(res);
});

app.MapGet("/tasks/{userId:guid}", async 
    (Guid userId, GetAllTaskQuery query, CancellationToken ct) =>
{
    var res = await query.ExecuteAsync(userId, ct);
    if (res == null || !res.Any())
    {
        return Results.NotFound();
    }
    return Results.Ok(res); 
});

app.MapPost("/task-comment", async (HttpContext context, CreateTaskCommentRequest request,
    CreateTaskCommentCommand command, IValidator<CreateTaskCommentRequest> validator,
    CancellationToken ct) => 
{
    var validationResult = validator.Validate(request);
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
    var res = await command.ExecuteAsync(request, ct);
    return Results.Ok(res);
});

app.MapGet("/task-comment/{id:guid}", async 
    (Guid id, GetByIdTaskCommentQuery query, CancellationToken ct) =>
{
    var res = await query.ExecuteAsync(id, ct);
    if (res == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(res);
});

app.Run();

