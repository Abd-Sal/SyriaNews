namespace SyriaNews.Validations;

public class ResendEmailConfirmationRequestValidations : AbstractValidator<ResendEmailConfirmationRequest>
{
    public ResendEmailConfirmationRequestValidations()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .When(x => x.Email is not null);
    }
}
