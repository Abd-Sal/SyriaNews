namespace SyriaNews.Validations;

public class TagRequestValidations : AbstractValidator<TagRequest>
{
    public TagRequestValidations()
    {
        RuleFor(x => x.TagName)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .Length(1, 256)
            .When(x => x.TagName is not null)
            .WithMessage("{PropertyName} length should be between ({MinLength}, {MaxLength})")
            .Must(x => checkTag(x))
            .WithMessage("Tag syntax is wrong");
    }
    private bool checkTag(string tag)
        => tag is null
        ? false
        : tag.StartsWith("#") &&
          !tag.Contains(" ") &&
          !"123456789".Contains(tag[0]) &&
          tag.Any(c => "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".Contains(c)) &&
          !tag.Any(c => "*&@!%$^()-+=[]{}:;'\"\\/|<>?".Contains(c));
}
