using BootCamp.Application.Feature.Task.Command;
using BootCamp.Application.Feature.Task.Models.Request;
using BootCamp.Application.Feature.Task.Query;
using FluentValidation;
using Wk1.ProblemDetails;

namespace Wk1.Endpoints;

public static class TaskEndpoint
{
    public static void MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
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

        app.MapPatch("/task/{id:guid}", async
        (HttpContext context, IValidator<UpdateTaskRequest> validator,
        Guid id, UpdateTaskRequest request, UpdateTaskCommand command, CancellationToken ct) =>
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
            var res = await command.ExecuteAsync(id, request, ct);
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

        app.MapGet("/task/{id:guid}", async
        (Guid id, GetByIdTaskQuery query, CancellationToken ct) =>
        {
            var res = await query.ExecuteAsync(id, ct);
            return Results.Ok(res);
        });
    }
}
