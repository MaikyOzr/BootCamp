using BootCamp.UserService.Application.Auth.Models.Request;
using BootCamp.UserService.Application.Auth.Models.Response;
using BootCamp.UserService.Database;
using BootCamp.UserService.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BootCamp.UserService.Application.Auth;

public sealed class SignInCommand(UserDbContext context, JwtTokenService jwtService)
{
    public async Task<SignInResponse> ExecuteAsync(SignInRequst request, CancellationToken ct)
    {
        var existingUser = await context.Users.Where(x=> x.Email == request.Email).SingleOrDefaultAsync(ct);
        bool isTruePassword = await context.Users.AnyAsync(x => x.Password == request.Password, ct);
        if (!isTruePassword)
        {
            throw new Exception("Wrong Password!");
        }

        var claims = new List<Claim> 
        {
            new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString()),
            new Claim(ClaimTypes.Email, existingUser.Email),
            new Claim(ClaimTypes.Name, existingUser.FirstName)
        };

        var token = jwtService.CreateAccessToken(claims);

        return new() { Id = existingUser.Id, Token = token };

    }
}
