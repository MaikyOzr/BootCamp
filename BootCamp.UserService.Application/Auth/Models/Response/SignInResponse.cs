namespace BootCamp.UserService.Application.Auth.Models.Response;

public sealed record SignInResponse 
{
    public Guid Id { get; init; }
    public string Token { get; init; }
};
