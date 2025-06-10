namespace SyriaNews.Validations;

public class MemberRequestValidations : AbstractValidator<MemberRequest>
{
    public MemberRequestValidations()
    {
        RuleFor(x => x.FirstName)
           .NotEmpty()
           .WithMessage("{PropertyName} is null/empty")
           .Length(1, 256)
           .When(x => x.FirstName is not null)
           .WithMessage("{PropertyName} length should be between ({MinLength}, {MaxLength})");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .Length(1, 256)
            .When(x => x.LastName is not null)
            .WithMessage("{PropertyName} length should be between ({MinLength}, {MaxLength})");

        RuleFor(x => x.Gender)
            .NotEmpty();
    }
}

