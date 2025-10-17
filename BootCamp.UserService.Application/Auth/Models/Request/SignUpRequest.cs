namespace BootCamp.UserService.Application.Auth.Models.Request;

public sealed record SignUpRequest
    (string Email, string FirstName, string LastName, string Password);
