using BootCamp.Application.Feature.Auth.Models.Request;
using BootCamp.Application.Feature.Auth.SingIn.Command;
using FluentValidation;

namespace Wk1.Endpoints;

internal static class AuthEndpoint
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/user", async
        (HttpContext context, SingInRequst request, SignInCommand command,
        IValidator<SingInRequst> validator, CancellationToken ct) =>
        {
            validator.ValidateAndThrow(request);
            var res = await command.ExecuteAsync(request, ct);

            return Results.Ok(res);
        });
    }
}
