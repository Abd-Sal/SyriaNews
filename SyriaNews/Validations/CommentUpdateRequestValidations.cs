namespace SyriaNews.Validations;

public class CommentUpdateRequestValidations : AbstractValidator<CommentUpdateRequest>
{
    public CommentUpdateRequestValidations()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .Length(1, 2000)
            .When(x => x.Content is not null)
            .WithMessage("{PropertyName} length should be between ({MinLength}, {MaxLength})");
    }
}



