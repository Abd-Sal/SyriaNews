namespace SyriaNews.DTOs.Follower;

public record FollowerResponse(
    string Id,
    string NewsPaperID,
    string MemberID,
    DateTime Date
);
