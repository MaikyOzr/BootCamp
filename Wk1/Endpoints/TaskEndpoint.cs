using BootCamp.Application.Feature.Task.Command;
using BootCamp.Application.Feature.Task.Models.Request;
using BootCamp.Application.Feature.Task.Models.Response;
using BootCamp.Application.Feature.Task.Query;
using BootCamp.RabitMqPublisher;
using FluentValidation;
using Microsoft.Extensions.Caching.Memory;
using RabbitMQ.Client;
using StackExchange.Redis;
using System.Text.Json;

namespace Wk1.Endpoints;

public static class TaskEndpoint
{
    public static void MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/task", async (HttpContext context, CreateTaskRequest request,
        CreateTaskCommand command, IValidator<CreateTaskRequest> validator, 
        IMessagePublisher publisher, CancellationToken ct) =>
        {
            validator.ValidateAndThrow(request);
            await publisher.PublishAsync("task", request, ct);
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
        (Guid userId, GetAllTaskQuery query, IMemoryCache cache, CancellationToken ct) =>
        {
            string cacheKey = $"tasks";
            if (cache.Get(cacheKey) == null)
            {
                var res = await query.ExecuteAsync(userId, ct);
                if (res == null || !res.Any())
                {
                    return Results.NotFound();
                }
                cache.Set(cacheKey, res, TimeSpan.FromMinutes(10));
            }
            var tasks = cache.Get(cacheKey);
            return Results.Ok(tasks);
        });

        app.MapGet("/task/{id:guid}", async
        (Guid id, GetByIdTaskQuery query, HttpClient client, IConnectionMultiplexer muxer, CancellationToken ct) =>
        {
            var cacheKey = $"task:{id}";
            IDatabase redis = muxer.GetDatabase();
            var value = await redis.StringGetAsync(cacheKey);
            if (value.IsNull)
            {
                var res = await query.ExecuteAsync(id, ct);
                await redis.StringSetAsync(cacheKey, JsonSerializer.Serialize(res), TimeSpan.FromMinutes(10));
                return Results.Ok(res);
            }
            var task = JsonSerializer.Deserialize<GetByIdTaskResponse>(value.ToString());
            return Results.Ok(task);

        });

        app.MapDelete("/task/{id:guid}", async (Guid id, DeleteTaskQuery query, CancellationToken ct) => 
        {
            var res = await query.ExecuteAsync(id, ct);
            return Results.Ok(res);
        });
    }
}
