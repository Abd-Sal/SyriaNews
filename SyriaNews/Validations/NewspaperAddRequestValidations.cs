namespace SyriaNews.Validations;

public class NewspaperAddRequestValidations : AbstractValidator<NewspaperAddRequest>
{
    public NewspaperAddRequestValidations()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .Length(1, 256)
            .When(x => x.Name is not null)
            .WithMessage("{PropertyName} length should be between ({MinLength}, {MaxLength})");

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


