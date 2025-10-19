using BootCamp.TaskService.Application.TaskFeature.Models.Request;
using FluentValidation;

namespace BootCamp.TaskService.Application.Validation;

public class CreateTaskWithFirstCommentValidator : AbstractValidator<CreateTaskWithFirstCommentRequest>
{
    public CreateTaskWithFirstCommentValidator()
    {
        RuleFor(x => x.TaskTitle)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters");
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
        RuleFor(x => x.TaskComment)
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(500).WithMessage("Content must not exceed 500 characters");
    }
}
