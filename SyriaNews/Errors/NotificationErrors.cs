namespace SyriaNews.Errors;

public class NotificationErrors
{
    public static readonly Error NotificationNotFound = 
        new("Notification.NotFound",
            "notification is not found!",
            StatusCodes.Status404NotFound);
    
    public static readonly Error NotAllowedNotification = 
        new("Notification.NotAllowedToRead",
            "this notification is not allowed for you to read!",
            StatusCodes.Status400BadRequest);

    public static readonly Error NotificationAlreadyReaded = 
        new("Notification.AlreadyReaded",
            "this notification already readed!",
            StatusCodes.Status400BadRequest);
}
