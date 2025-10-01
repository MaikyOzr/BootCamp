using BootCamp.Application.Feature.Task.Models.Request;
using FluentValidation;

namespace BootCamp.Application.ValidationService;

public class CreateTaskRequestValidation : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidation()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters");
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}
