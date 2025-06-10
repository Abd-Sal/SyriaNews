namespace SyriaNews.Validations;

public class ReportForArticleRequestValidations : AbstractValidator<ReportForArticleRequest>
{
    public ReportForArticleRequestValidations()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .Length(1, 256)
            .When(x => x.Title is not null)
            .WithMessage("{PropertyName} length should be between ({MinLength}, {MaxLength})");
        
        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .Length(1, 256)
            .When(x => x.Message is not null)
            .WithMessage("{PropertyName} length should be between ({MinLength}, {MaxLength})");
     
        RuleFor(x => x.ArticleID)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .Length(1, 256)
            .When(x => x.ArticleID is not null)
            .WithMessage("{PropertyName} length should be between ({MinLength}, {MaxLength})");

    }
}
