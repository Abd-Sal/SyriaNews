namespace SyriaNews.DTOs.Follower;

public record FullFollowerResponse(
    string Id,
    NewspaperBreifResponse NewsPaper,
    MemberBreifResponse Member,
    DateTime Date
);
