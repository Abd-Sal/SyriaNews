namespace SyriaNews.DTOs.Member;

public record MemberRequest(
    string FirstName,
    string LastName,
    bool Gender    
);
