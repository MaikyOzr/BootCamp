namespace BootCamp.Application.Feature.Auth.Models.Request;

public sealed record SignUpRequest
    (string Email, string FirstName, string LastName, string Password);
