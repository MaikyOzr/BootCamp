using BootCamp.Application.AuthService;
using BootCamp.Application.Feature.Auth.Models.Request;
using BootCamp.Application.Feature.Auth.SingIn.Command;
using BootCamp.Application.Feature.Auth.SingUp.Command;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Wk1.Endpoints;

internal static class AuthEndpoint
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/sign-in", async
        ([FromServices] JwtTokenService service, [FromBody]SignInRequst request, [FromServices]SignInCommand command,
        [FromServices]IValidator<SignInRequst> validator, CancellationToken ct) =>
        {
            validator.ValidateAndThrow(request);
            var res = await command.ExecuteAsync(request, ct);
            return Results.Ok(res);
        }).AllowAnonymous();

        app.MapPost("/auth/sign-up", async 
            ([FromServices]SignUpCommand command, [FromBody]SignUpRequest request,
            [FromServices]IValidator<SignUpRequest> validator, CancellationToken ct) => 
        { 
                validator.ValidateAndThrow(request);
                var res = await command.ExecuteAsync(request, ct);
                return Results.Ok(res);
        }).AllowAnonymous();
    }
}
