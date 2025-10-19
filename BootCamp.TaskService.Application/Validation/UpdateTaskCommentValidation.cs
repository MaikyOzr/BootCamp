using BootCamp.TaskService.Application.TaskCommentFeature.Models.Request;
using FluentValidation;

namespace BootCamp.TaskService.Application.Validation;

public class UpdateTaskCommentValidation : AbstractValidator<UpdateTaskCommentRequest>
{
    public UpdateTaskCommentValidation()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(1000).WithMessage("Content must not exceed 1000 characters");
    }
}
