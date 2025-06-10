namespace SyriaNews.Validations;

public class CategoryRequestValidations : AbstractValidator<CategoryRequest>
{
    public CategoryRequestValidations()
    {
        RuleFor(x => x.CategoryName)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .Length(1, 256)
            .When(x => x.CategoryName is not null)
            .WithMessage("{PropertyName} length should be between ({MinLength}, {MaxLength})");
    }
}





