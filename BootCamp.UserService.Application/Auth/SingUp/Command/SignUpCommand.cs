using BootCamp.UserService.Application.Auth.Models.Request;
using BootCamp.UserService.Database;
using BootCamp.UserService.Domain;
using Microsoft.AspNetCore.Identity;

namespace BootCamp.UserService.Application.Auth;

public sealed class SignUpCommand(UserDbContext context)
{
    public async Task<Guid> ExecuteAsync(SignUpRequest request, CancellationToken ct)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            LastName = request.LastName,
            FirstName = request.FirstName,
            Password = request.Password,
        };

        context.Users.Add(user);
        await context.SaveChangesAsync(ct);
        return user.Id;
    }
}
