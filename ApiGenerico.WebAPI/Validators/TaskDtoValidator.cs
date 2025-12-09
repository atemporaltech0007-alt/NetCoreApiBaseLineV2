using ApiGenerico.Domain.Models.Dto;
using FluentValidation;

namespace ApiGenerico.WebAPI.Validators;

public class CreateTaskDtoValidator : AbstractValidator<CreateTaskDto>
{
    public CreateTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(4000).WithMessage("Description cannot exceed 4000 characters");

        RuleFor(x => x.StateId)
            .GreaterThan(0).WithMessage("StateId must be greater than 0");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow.AddDays(-1))
            .When(x => x.DueDate.HasValue)
            .WithMessage("DueDate must be in the future");
    }
}

public class UpdateTaskDtoValidator : AbstractValidator<UpdateTaskDto>
{
    public UpdateTaskDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(4000).WithMessage("Description cannot exceed 4000 characters");

        RuleFor(x => x.StateId)
            .GreaterThan(0).WithMessage("StateId must be greater than 0");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow.AddDays(-1))
            .When(x => x.DueDate.HasValue)
            .WithMessage("DueDate must be in the future");

        RuleFor(x => x.RowVersion)
            .NotNull().WithMessage("RowVersion is required for concurrency control");
    }
}
