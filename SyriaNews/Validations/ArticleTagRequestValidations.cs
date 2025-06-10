namespace SyriaNews.Validations;

public class ArticleTagRequestValidations : AbstractValidator<ArticleTagRequest>
{
    public ArticleTagRequestValidations()
    {
        RuleFor(x => x.ArticleID)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .Length(1, 150)
            .When(x => x.ArticleID is not null)
            .WithMessage("{PropertyName} length should be between ({MinLength}, {MaxLength})");

        RuleFor(x => x.TagID)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .Length(1, 150)
            .When(x => x.TagID is not null)
            .WithMessage("{PropertyName} length should be between ({MinLength}, {MaxLength})");

    }
}

