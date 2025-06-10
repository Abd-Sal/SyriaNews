namespace SyriaNews.Validations;

public class AdminRequestValidations : AbstractValidator<AdminRequest>
{
    public AdminRequestValidations()
    {
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


