namespace SyriaNews.Validations;

public class FollowerRequestValidations : AbstractValidator<FollowerRequest>
{
    public FollowerRequestValidations()
    {
        RuleFor(x => x.NewsPaperID)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .Length(1, 150)
            .When(x => x.NewsPaperID is not null)
            .WithMessage("{PropertyName} length should be between ({MinLength}, {MaxLength})");

    }
}

