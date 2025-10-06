using BootCamp.Application.Feature.TaskCommentFeature.Command;
using BootCamp.Application.Feature.TaskCommentFeature.Models.Request;
using BootCamp.Application.Feature.TaskCommentFeature.Query;
using FluentValidation;

namespace Wk1.Endpoints;

public static class TaskCommentEndpoint
{
    public static void MapTaskCommentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/task-comment", async (HttpContext context, CreateTaskCommentRequest request,
        CreateTaskCommentCommand command, IValidator<CreateTaskCommentRequest> validator,
        CancellationToken ct) =>
        {
            validator.ValidateAndThrow(request);
            var res = await command.ExecuteAsync(request, ct);
            return Results.Ok(res);
        });

        app.MapPatch("/task-comment/{id:guid}", async (
        HttpContext context, IValidator<UpdateTaskCommentRequest> validator,
        Guid id, UpdateTaskCommentRequest request, UpdateTaskCommentCommand command, CancellationToken ct) =>
        {
            validator.ValidateAndThrow(request);
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

        app.MapDelete("/task-comment/{id:guid}",async (Guid id, DeleteTaskCommentQuery query, CancellationToken ct) => 
        {
            var res = await query.ExecuteAsync(id, ct);
            return Results.Ok(res);
        });
    }
}
