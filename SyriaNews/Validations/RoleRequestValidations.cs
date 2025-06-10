namespace SyriaNews.Validations;

public class RoleRequestValidations : AbstractValidator<RoleRequest>
{
    public RoleRequestValidations()
    {
        RuleFor(x => x.name)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .Length(1, 255)
            .When(x => x.name is not null)
            .WithMessage("{PropertyName} length should be between ({MinLength}, {MaxLength})");
    }
}
