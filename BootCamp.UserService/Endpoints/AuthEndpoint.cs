using BootCamp.UserService.Application;
using BootCamp.UserService.Application.Auth;
using BootCamp.UserService.Application.Auth.Models.Request;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BootCamp.UserService.Web;

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
