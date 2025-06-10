namespace SyriaNews.Validations;

public class ReportForNewspaperRequestValidations : AbstractValidator<ReportForNewspaperRequest>
{
    public ReportForNewspaperRequestValidations()
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
     
        RuleFor(x => x.NewspaperID)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .Length(1, 256)
            .When(x => x.NewspaperID is not null)
            .WithMessage("{PropertyName} length should be between ({MinLength}, {MaxLength})");

    }
}
