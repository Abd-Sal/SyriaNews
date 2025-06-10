namespace SyriaNews.Validations;

public class LikeRequestValidations : AbstractValidator<LikeRequest>
{
    public LikeRequestValidations()
    {
        RuleFor(x => x.ArticleID)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .Length(1, 150)
            .When(x => x.ArticleID is not null)
            .WithMessage("{PropertyName} length should be between ({MinLength}, {MaxLength})");

    }
}

