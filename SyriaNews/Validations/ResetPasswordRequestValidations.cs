namespace SyriaNews.Validations;

public class ResetPasswordRequestValidations : AbstractValidator<ResetPasswordRequest> 
{
    public ResetPasswordRequestValidations()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .EmailAddress()
            .When(x => x.Email is not null);

        RuleFor(x => x.Code)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .Must(x => HelpToolsExtensions.PasswordFormat(x))
            .When(x => x.NewPassword is not null)
            .WithMessage("{PropertyName} should contain(A,a,@,1)");
    }
}
