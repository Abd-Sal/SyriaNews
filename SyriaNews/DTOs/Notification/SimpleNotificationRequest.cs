namespace SyriaNews.DTOs.Notification;

public record SimpleNotificationRequest(
    string EntityID,
    string Title,
    string Message
);
