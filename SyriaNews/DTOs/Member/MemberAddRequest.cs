namespace SyriaNews.DTOs.Member;

public record MemberAddRequest(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    bool Gender
);
