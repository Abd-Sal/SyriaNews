namespace SyriaNews.Validations;

public class RefreshTokenRequestValidations : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenRequestValidations()
    {
        RuleFor(x => x.Token)
           .NotEmpty()
           .WithMessage("{PropertyName} is null/empty");

        RuleFor(x => x.RefreshToken)
           .NotEmpty()
           .WithMessage("{PropertyName} is null/empty");
    }
}


