using BootCamp.Application.Feature.Auth.Models.Request;
using BootCamp.Application.Feature.BaseResponse;
using BootCamp.Domain;
using BootCamp.Infrastruture;

namespace BootCamp.Application.Feature.Auth.SingIn.Command;

public class SignInCommand(AppDbContext context)
{
    public async Task<BaseApiResponse> ExecuteAsync(SingInRequst requst, CancellationToken ct)
    {
        var user = new User() 
        {
            Email = requst.Email, 
            FirstName = requst.FirstName, 
            LastName= requst.LastName,
            PasswordHash = requst.Password
        };

        context.Add(user);
        await context.SaveChangesAsync(ct);

        return new() { Id = user.Id };
    }
}
