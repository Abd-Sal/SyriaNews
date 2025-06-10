namespace SyriaNews.Validations;

public class ArticleImageRequestValidations : AbstractValidator<ImageRequest>
{
    public ArticleImageRequestValidations()
    {
        RuleFor(x => x.Placement)
            .NotEmpty();
    }
}
