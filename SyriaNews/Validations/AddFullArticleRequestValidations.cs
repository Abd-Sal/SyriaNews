namespace SyriaNews.Validations;

public class AddFullArticleRequestValidations : AbstractValidator<AddFullArticleRequest>
{
    public AddFullArticleRequestValidations()
    {
        RuleFor(x => x.ArticleRequest).NotNull().SetValidator(new ArticleRequestValidations());

        RuleFor(x => x.Tags)
            .Must(tags => tags.Count <= 50)
            .When(x => x.Tags is not null)
            .WithMessage("Maximum 50 tags allowed")
            .ForEach(tag => tag.SetValidator(new TagRequestValidations()));

        RuleFor(x => x.Images)
            .Must(images => images.Count <= 50).WithMessage("Maximum 50 images allowed")
            .When(x => x.Images is not null)
            .ForEach(image => image.SetValidator(new FullImageRequestValidations()))
            .When(x => x.Images is not null);
    }
}

