using BootCamp.Application.Feature.Auth.Models.Request;
using FluentValidation;

namespace BootCamp.Application.Services.ValidationService;

public class SignInRequestValidation : AbstractValidator<SignInRequst>
{
    public SignInRequestValidation()
    {
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");
        RuleFor(user => user.Password)
            .NotNull().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }
}
