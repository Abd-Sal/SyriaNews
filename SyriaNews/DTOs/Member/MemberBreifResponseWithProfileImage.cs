namespace SyriaNews.DTOs.Member;

public record MemberBreifResponseWithProfileImage(
    MemberBreifResponse Member,
    ProfileImageResponse? ProfileImage
);
