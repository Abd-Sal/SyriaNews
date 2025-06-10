namespace SyriaNews.DTOs.Member;

public record MemberBreifResponse
{
    public string Id { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Gender { get; init; } = "Male";
    public bool IsActive { get; init; } = false;
};
