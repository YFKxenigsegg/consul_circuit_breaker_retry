namespace CCBR.Services.NotificationService;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Type { get; set; }
    public required string Title { get; set; }
    public required string Message { get; set; }
    public NotificationChannel Channel { get; set; }
    public NotificationStatus Status { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? ReadAt { get; set; }
    public string? Metadata { get; set; } // JSON for additional data
    public int? RelatedEntityId { get; set; } // Order ID, Product ID, etc.
    public required string RelatedEntityType { get; set; } // "Order", "Product", etc.
}

public enum NotificationChannel
{
    Email,
    Push,
    InApp
}

public enum NotificationStatus
{
    Pending,
    Sent,
    Failed,
    Delivered,
    Read
}

public class NotificationTemplate
{
    public int Id { get; set; }
    public required string Type { get; set; }
    public NotificationChannel Channel { get; set; }
    public required string Subject { get; set; }
    public required string Template { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateNotificationRequest
{
    public int UserId { get; set; }
    public required string Type { get; set; }
    public required string Title { get; set; }
    public required string Message { get; set; }
    public NotificationChannel Channel { get; set; } = NotificationChannel.InApp;
    public Dictionary<string, object> Data { get; set; } = new();
    public int? RelatedEntityId { get; set; }
    public required string RelatedEntityType { get; set; }
}

public class SendEmailRequest
{
    public required string To { get; set; }
    public required string Subject { get; set; }
    public required string Body { get; set; }
    public bool IsHtml { get; set; } = true;
    public List<string> Attachments { get; set; } = new();
}

public class OrderNotificationData
{
    public int OrderId { get; set; }
    public required string OrderNumber { get; set; }
    public decimal TotalAmount { get; set; }
    public required string Status { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItemData> Items { get; set; } = new();
}

public class OrderItemData
{
    public required string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}