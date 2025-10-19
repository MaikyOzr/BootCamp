using BootCamp.TaskService.Application.TaskFeature.Models.Request;
using FluentValidation;

namespace BootCamp.TaskService.Application.Validation;

public class UpdateTaskRequestValidation : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidation()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters");
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
    }
}
