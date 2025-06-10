namespace SyriaNews.DTOs.Notification;

public record NotificationResponse(
    string Id,
    string Title,
    string Message,
    string NotificationType,
    string? EntityID,
    string? EntityType,
    DateTime Date,
    bool IsReaded
);