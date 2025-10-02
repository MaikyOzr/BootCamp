using BootCamp.Application.Feature.Auth.Models.Request;
using BootCamp.Application.Feature.Auth.SingIn.Command;
using FluentValidation;
using Wk1.ProblemDetails;

namespace Wk1.Endpoints;

public static class AuthEndpoint
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
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
    }
}
