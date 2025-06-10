namespace SyriaNews.Validations;

public class NewspaperRequestValidations : AbstractValidator<NewsPaperRequest>
{
    public NewspaperRequestValidations()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .Length(1, 256)
            .When(x => x.Name is not null)
            .WithMessage("{PropertyName} length should be between ({MinLength}, {MaxLength})");
    }
}


