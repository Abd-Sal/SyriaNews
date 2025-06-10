namespace SyriaNews.Validations;

public class ConfirmationEmailRequestValidations : AbstractValidator<ConfirmationRequest>
{
    public ConfirmationEmailRequestValidations()
    {
        RuleFor(x => x.UserID)
            .NotEmpty();
        
        RuleFor(x => x.Code)
            .NotEmpty();
    }
}
