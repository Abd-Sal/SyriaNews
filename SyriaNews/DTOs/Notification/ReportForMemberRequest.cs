namespace SyriaNews.DTOs.Notification;

public record ReportForMemberRequest(
    string MemberID,
    string Title,
    string Message
);
