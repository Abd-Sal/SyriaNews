namespace SyriaNews.Validations;

public class CommentRequestValidations : AbstractValidator<CommentRequest>
{
    public CommentRequestValidations()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .Length(1, 2000)
            .When(x => x.Content is not null)
            .WithMessage("{PropertyName} length should be between ({MinLength}, {MaxLength})");

        RuleFor(x => x.ArticleID)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .Length(1, 150)
            .When(x => x.ArticleID is not null)
            .WithMessage("{PropertyName} length should be between ({MinLength}, {MaxLength})");

    }
}





