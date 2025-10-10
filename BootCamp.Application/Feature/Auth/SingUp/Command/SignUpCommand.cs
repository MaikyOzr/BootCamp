using BootCamp.Application.AuthService;
using BootCamp.Application.Feature.Auth.Models.Request;
using BootCamp.Domain;
using Microsoft.AspNetCore.Identity;

namespace BootCamp.Application.Feature.Auth.SingUp.Command;

public sealed class SignUpCommand(UserManager<User> userManager)
{
    public async Task<Guid> ExecuteAsync(SignUpRequest request, CancellationToken ct)
    {

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.FirstName
        };

        await userManager.CreateAsync(user, request.Password);

        return user.Id;
    }
}
