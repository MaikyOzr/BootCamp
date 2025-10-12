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
            UserName = request.FirstName,
            LastName = request.LastName,
            FirstName = request.FirstName
        };

        var res = await userManager.CreateAsync(user, request.Password);

        if (res.Succeeded) 
        {
            return user.Id;
        }
        throw new Exception(string.Join(", ", res.Errors.Select(e => e.Description)));
    }
}
