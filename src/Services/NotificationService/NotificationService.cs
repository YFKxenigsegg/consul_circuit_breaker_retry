using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace CCBR.Services.NotificationService;

public interface INotificationService
{
    Task<Notification> CreateNotificationAsync(CreateNotificationRequest request);
    Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId, bool unreadOnly = false);
    Task<bool> MarkAsReadAsync(int notificationId);
    Task<bool> MarkAllAsReadAsync(int userId);
    Task<int> GetUnreadCountAsync(int userId);
    Task ProcessPendingNotificationsAsync();
}

public class NotificationService(
    NotificationDbContext context,
    IEmailService emailService,
    IPushNotificationService pushService,
    ITemplateRenderer templateRenderer,
    ILogger<NotificationService> logger) : INotificationService
{
    private readonly NotificationDbContext _context = context;
    private readonly IEmailService _emailService = emailService;
    private readonly IPushNotificationService _pushService = pushService;
    private readonly ITemplateRenderer _templateRenderer = templateRenderer;
    private readonly ILogger<NotificationService> _logger = logger;

    public async Task<Notification> CreateNotificationAsync(CreateNotificationRequest request)
    {
        _logger.LogInformation("Creating notification for user {UserId}, type: {Type}",
            request.UserId, request.Type);

        var notification = new Notification
        {
            UserId = request.UserId,
            Type = request.Type,
            Title = request.Title,
            Message = request.Message,
            Channel = request.Channel,
            Status = NotificationStatus.Pending,
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
            Metadata = JsonSerializer.Serialize(request.Data),
            RelatedEntityId = request.RelatedEntityId,
            RelatedEntityType = request.RelatedEntityType
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        // Process notification immediately for real-time channels
        if (request.Channel == NotificationChannel.InApp || request.Channel == NotificationChannel.Push)
        {
            await ProcessNotificationAsync(notification);
        }

        return notification;
    }

    public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(int userId, bool unreadOnly = false)
    {
        var query = _context.Notifications.Where(n => n.UserId == userId);

        if (unreadOnly)
        {
            query = query.Where(n => !n.IsRead);
        }

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Take(50)
            .ToListAsync();
    }

    public async Task<bool> MarkAsReadAsync(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification == null) return false;

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        notification.Status = NotificationStatus.Read;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAllAsReadAsync(int userId)
    {
        var unreadNotifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            notification.Status = NotificationStatus.Read;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetUnreadCountAsync(int userId)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId && !n.IsRead);
    }

    public async Task ProcessPendingNotificationsAsync()
    {
        var pendingNotifications = await _context.Notifications
            .Where(n => n.Status == NotificationStatus.Pending)
            .OrderBy(n => n.CreatedAt)
            .Take(100) // Process in batches
            .ToListAsync();

        foreach (var notification in pendingNotifications)
        {
            await ProcessNotificationAsync(notification);
        }
    }

    private async Task ProcessNotificationAsync(Notification notification)
    {
        try
        {
            switch (notification.Channel)
            {
                case NotificationChannel.Email:
                    await SendEmailNotificationAsync(notification);
                    break;
                case NotificationChannel.Push:
                    await SendPushNotificationAsync(notification);
                    break;
                case NotificationChannel.InApp:
                    // In-app notifications are already stored, just mark as sent
                    notification.Status = NotificationStatus.Sent;
                    notification.SentAt = DateTime.UtcNow;
                    break;
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully processed notification {NotificationId}", notification.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process notification {NotificationId}", notification.Id);
            notification.Status = NotificationStatus.Failed;
            await _context.SaveChangesAsync();
        }
    }

    private async Task SendEmailNotificationAsync(Notification notification)
    {
        // Get user email (this would come from User service)
        var userEmail = await GetUserEmailAsync(notification.UserId);
        if (string.IsNullOrEmpty(userEmail))
        {
            throw new InvalidOperationException($"No email found for user {notification.UserId}");
        }

        // Get template
        var template = await GetTemplateAsync(notification.Type, NotificationChannel.Email);
        if (template == null)
        {
            // Use notification content directly if no template
            await _emailService.SendEmailAsync(userEmail, notification.Title, notification.Message);
        }
        else
        {
            // Render template with data
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(notification.Metadata ?? "{}");
            var subject = _templateRenderer.Render(template.Subject, data);
            var body = _templateRenderer.Render(template.Template, data);

            await _emailService.SendEmailAsync(userEmail, subject, body);
        }

        notification.Status = NotificationStatus.Sent;
        notification.SentAt = DateTime.UtcNow;
    }

    private async Task SendPushNotificationAsync(Notification notification)
    {
        // Get user device tokens (this would come from User service or device registration)
        var deviceTokens = await GetUserDeviceTokensAsync(notification.UserId);
        if (!deviceTokens.Any())
        {
            throw new InvalidOperationException($"No device tokens found for user {notification.UserId}");
        }

        await _pushService.SendPushNotificationAsync(deviceTokens, notification.Title, notification.Message);

        notification.Status = NotificationStatus.Sent;
        notification.SentAt = DateTime.UtcNow;
    }

    private async Task<NotificationTemplate> GetTemplateAsync(string type, NotificationChannel channel) =>
        await _context.NotificationTemplates
            .FirstOrDefaultAsync(t => t.Type == type && t.Channel == channel && t.IsActive);

    private async Task<string> GetUserEmailAsync(int userId)
    {
        // This would call the User service to get email
        // For now, return a placeholder
        return $"user{userId}@example.com";
    }

    private async Task<string> GetUserPhoneAsync(int userId)
    {
        // This would call the User service to get phone
        // For now, return a placeholder
        return "+1234567890";
    }

    private async Task<List<string>> GetUserDeviceTokensAsync(int userId)
    {
        // This would get device tokens from a user device registry
        // For now, return empty list
        return new List<string>();
    }
}
