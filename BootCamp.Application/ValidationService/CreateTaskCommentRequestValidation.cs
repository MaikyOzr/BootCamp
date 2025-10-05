using BootCamp.Application.Feature.TaskCommentFeature.Models.Request;
using FluentValidation;

namespace BootCamp.Application.ValidationService;

public class CreateTaskCommentRequestValidation 
    : AbstractValidator<CreateTaskCommentRequest>
{
    public CreateTaskCommentRequestValidation()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required")
            .MaximumLength(500).WithMessage("Content must not exceed 500 characters");
        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("TaskId is required");
    }
}
