namespace SyriaNews.Validations;

public class ForgetPasswordRequestValidations : AbstractValidator<ForgetPasswordRequest>
{
    public ForgetPasswordRequestValidations()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .When(x => x.Email is not null);
    }
}
