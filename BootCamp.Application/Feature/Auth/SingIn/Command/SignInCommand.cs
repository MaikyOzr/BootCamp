using BootCamp.Application.AuthService;
using BootCamp.Application.Feature.Auth.Models.Request;
using BootCamp.Application.Feature.Auth.Models.Response;
using BootCamp.Domain;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BootCamp.Application.Feature.Auth.SingIn.Command;

public sealed class SignInCommand(UserManager<User> userManager, JwtTokenService jwtService)
{
    public async Task<SignInResponse> ExecuteAsync(SignInRequst request, CancellationToken ct)
    {
        var existingUser = await userManager.FindByEmailAsync(request.Email);
        if (!await userManager.CheckPasswordAsync(existingUser, request.Password))
        {
            throw new Exception("Wrong Password!");
        }

        var claims = new List<Claim> 
        {
            new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()),
            new Claim(ClaimTypes.Email, existingUser.Email),
            new Claim(ClaimTypes.Name, existingUser.UserName)
        };

        var token = jwtService.CreateAccessToken(claims);

        return new() { Id = existingUser.Id, Token = token };

    }
}
