using BootCamp.Application.Feature.TaskCommentFeature.Command;
using BootCamp.Application.Feature.TaskCommentFeature.Models.Request;
using BootCamp.Application.Feature.TaskCommentFeature.Query;
using FluentValidation;
using Wk1.ProblemDetails;

namespace Wk1.Endpoints;

public static class TaskCommentEndpoint
{
    public static void MapTaskCommentEndpoints(this IEndpointRouteBuilder app)
    {
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

        app.MapPatch("/task-comment/{id:guid}", async (
        HttpContext context, IValidator<UpdateTaskCommentRequest> validator,
        Guid id, UpdateTaskCommentRequest request, UpdateTaskCommentCommand command, CancellationToken ct) =>
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

        app.MapGet("/task-comments/{id:guid}", async
        (Guid id, GetByIdTaskCommentQuery query, CancellationToken ct) =>
        {
            var res = await query.ExecuteAsync(id, ct);
            if (res == null)
            {
                return Results.NotFound();
            }
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
    }
}
