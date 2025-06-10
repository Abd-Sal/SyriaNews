namespace SyriaNews.Validations;

public class ArticleRequestValidations : AbstractValidator<ArticleRequest>
{
    public ArticleRequestValidations()
    {
        RuleFor(x => x.Title)
           .NotEmpty()
           .WithMessage("{PropertyName} is null/empty")
           .Length(1, 256)
           .When(x => x.Title is not null)
           .WithMessage("{PropertyName} length should be between ({MinLength}, {MaxLength})");
        
        RuleFor(x => x.Description)
           .NotEmpty()
           .WithMessage("{PropertyName} is null/empty")
           .Length(1, 500)
           .When(x => x.Description is not null)
           .WithMessage("{PropertyName} length should be between ({MinLength}, {MaxLength})");

        RuleFor(x => x.Content)
           .NotEmpty()
           .WithMessage("{PropertyName} is null/empty")
           .MinimumLength(550)
           .When(x => x.Content is not null)
           .WithMessage("{PropertyName} length should be more than ({MinLength})");

        RuleFor(x => x.CategoryID)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .Length(1, 150)
            .When(x => x.CategoryID is not null)
            .WithMessage("{PropertyName} length should be between ({MinLength}, {MaxLength})");
    }
}

