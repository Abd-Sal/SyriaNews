namespace SyriaNews.Validations;

public class LoginRequestValidations : AbstractValidator<LoginRequest>
{
    public LoginRequestValidations()
    {
        RuleFor(x => x.email)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .EmailAddress()
            .When(x => x.email is not null);

        RuleFor(x => x.password)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .Must(x => HelpToolsExtensions.PasswordFormat(x))
            .When(x => x.password is not null)
            .WithMessage("{PropertyName} should contain(A,a,@,1)");
    }
}


