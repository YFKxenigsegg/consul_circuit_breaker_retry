using Microsoft.AspNetCore.Mvc;

namespace CCBR.Services.NotificationService;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController(INotificationService notificationService) : ControllerBase
{
    private readonly INotificationService _notificationService = notificationService;

    [HttpPost]
    public async Task<ActionResult<Notification>> CreateNotification([FromBody] CreateNotificationRequest request)
    {
        try
        {
            var notification = await _notificationService.CreateNotificationAsync(request);
            return CreatedAtAction(nameof(GetUserNotifications),
                new { userId = notification.UserId }, notification);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<Notification>>> GetUserNotifications(
        int userId,
        [FromQuery] bool unreadOnly = false)
    {
        var notifications = await _notificationService.GetUserNotificationsAsync(userId, unreadOnly);
        return Ok(notifications);
    }

    [HttpGet("user/{userId}/unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount(int userId)
    {
        var count = await _notificationService.GetUnreadCountAsync(userId);
        return Ok(count);
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var result = await _notificationService.MarkAsReadAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpPut("user/{userId}/read-all")]
    public async Task<IActionResult> MarkAllAsRead(int userId)
    {
        await _notificationService.MarkAllAsReadAsync(userId);
        return NoContent();
    }
}