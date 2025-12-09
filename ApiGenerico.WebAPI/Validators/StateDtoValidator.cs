using ApiGenerico.Domain.Models.Dto;
using FluentValidation;

namespace ApiGenerico.WebAPI.Validators;

public class CreateStateDtoValidator : AbstractValidator<CreateStateDto>
{
    public CreateStateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");
    }
}

public class UpdateStateDtoValidator : AbstractValidator<UpdateStateDto>
{
    public UpdateStateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");
    }
}
