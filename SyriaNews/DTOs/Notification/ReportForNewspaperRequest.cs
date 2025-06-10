namespace SyriaNews.DTOs.Notification;

public record ReportForNewspaperRequest(
    string NewspaperID,
    string Title,
    string Message
);
