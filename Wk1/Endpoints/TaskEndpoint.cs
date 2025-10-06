using BootCamp.Application.Feature.Task.Command;
using BootCamp.Application.Feature.Task.Models.Request;
using BootCamp.Application.Feature.Task.Query;
using FluentValidation;

namespace Wk1.Endpoints;

public static class TaskEndpoint
{
    public static void MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/task", async (HttpContext context, CreateTaskRequest request,
        CreateTaskCommand command, IValidator<CreateTaskRequest> validator,
        CancellationToken ct) =>
        {
            validator.ValidateAndThrow(request);
            var res = await command.ExecuteAsync(request, ct);

            return Results.Ok(res);
        });

        app.MapPost("/task-with-comment", async 
            (HttpContext context, CreateTaskWithFirstCommentRequest request, 
            CreateTaskWithFirstCommentCommand command, 
            IValidator<CreateTaskWithFirstCommentRequest> validator, CancellationToken ct) => 
            {
                validator.ValidateAndThrow(request);
                var res = await command.ExecuteAsync(request, ct);

                return Results.Ok(res);
            }
        );

        app.MapPatch("/task/{id:guid}", async
        (HttpContext context, IValidator<UpdateTaskRequest> validator,
        Guid id, UpdateTaskRequest request, UpdateTaskCommand command, CancellationToken ct) =>
        {
            validator.ValidateAndThrow(request);
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

        app.MapDelete("/task/{id:guid}", async (Guid id, DeleteTaskQuery query, CancellationToken ct) => 
        {
            var res = await query.ExecuteAsync(id, ct);
            return Results.Ok(res);
        });
    }
}
