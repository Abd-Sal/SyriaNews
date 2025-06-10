namespace SyriaNews.Validations;

public class ProfileImageRequestValidations : AbstractValidator<ProfileImageRequest>
{
    public ProfileImageRequestValidations()
    {
        RuleFor(x => x.UserID)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .Length(1, 150)
            .When(x => x.UserID is not null)
            .WithMessage("{PropertyName} length should be between ({MinLength}, {MaxLength})");
    }
}
