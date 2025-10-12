using BootCamp.Application.Feature.TaskCommentFeature.Models.Request;
using FluentValidation;

namespace BootCamp.Application.Services.ValidationService
{
    public class UpdateTaskCommentValidation : AbstractValidator<UpdateTaskCommentRequest>
    {
        public UpdateTaskCommentValidation()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required")
                .MaximumLength(1000).WithMessage("Content must not exceed 1000 characters");
        }
    }
}
