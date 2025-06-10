namespace SyriaNews.DTOs.Notification;

public record ReportForArticleRequest(
    string ArticleID,
    string Title,
    string Message
);
