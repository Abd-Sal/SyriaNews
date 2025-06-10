namespace SyriaNews.Validations;

public class MemberAddRequestValidations : AbstractValidator<MemberAddRequest>
{
    public MemberAddRequestValidations()
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
            .NotNull();

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .EmailAddress()
            .When(x => x.Email is not null);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .Must(x => HelpToolsExtensions.PasswordFormat(x))
            .When(x => x.Password is not null)
            .WithMessage("{PropertyName} should contain(A,a,@,1)");

    }
}

