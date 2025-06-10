namespace SyriaNews.Validations;

public class FullImageRequestValidations : AbstractValidator<FullImageRequest>
{
    public FullImageRequestValidations()
    {
        RuleFor(x => x.Placement)
            .NotEmpty()
            .Must(x => x > 0)
            .When(x => x is not null)
            .WithMessage("the placement should be > 0");
        
        RuleFor(x => x.File)
            .NotEmpty()
            .WithMessage("the file is required");
    }
}
