using BootCamp.TaskService.Application.TaskFeature.Models.Request;
using FluentValidation;

namespace BootCamp.TaskService.Application.Validation;

public class CreateTaskRequestValidation : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidation()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters");
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}
