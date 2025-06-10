namespace SyriaNews.Validations;

public class ResultFilterRequestValidations : AbstractValidator<ResultFilter>
{
    public ResultFilterRequestValidations()
    {
        RuleFor(x => x.PageSize)
            .LessThanOrEqualTo(50)
            .GreaterThanOrEqualTo(1);
        
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);
    }
}
