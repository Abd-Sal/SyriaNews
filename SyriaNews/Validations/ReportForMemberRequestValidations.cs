namespace SyriaNews.Validations;

public class ReportForMemberRequestValidations : AbstractValidator<ReportForMemberRequest>
{
    public ReportForMemberRequestValidations()
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
     
        RuleFor(x => x.MemberID)
            .NotEmpty()
            .WithMessage("{PropertyName} is null/empty")
            .Length(1, 256)
            .When(x => x.MemberID is not null)
            .WithMessage("{PropertyName} length should be between ({MinLength}, {MaxLength})");

    }
}
