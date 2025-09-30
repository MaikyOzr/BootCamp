using BootCamp.Application.Feature.Auth.Models.Request;
using FluentValidation;

namespace BootCamp.Application.Feature.ValidationService;

public class SignInRequestValidation : AbstractValidator<SingInRequst>
{
    public SignInRequestValidation()
    {
        RuleFor(user => user.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");
        RuleFor(user => user.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");
        RuleFor(user => user.Password)
            .NotNull().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }
}
