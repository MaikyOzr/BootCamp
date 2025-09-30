using BootCamp.Application.Feature.Auth.Models.Request;
using BootCamp.Application.Feature.BaseResponse;
using BootCamp.Domain;

namespace BootCamp.Application.Feature.Auth.SingIn.Command;

public class SignInCommand()
{
    public async Task<BaseApiResponse> ExecuteAsync( SingInRequst requst, CancellationToken ct)
    {
        var user = new User() 
        {
            Email = requst.Email, 
            FirstName = requst.FirstName, 
            LastName= requst.LastName,
            PasswordHash = requst.Password
        };

        user.Users.Add(user);
        
        return new() { Id = user.Id };
    }
}
