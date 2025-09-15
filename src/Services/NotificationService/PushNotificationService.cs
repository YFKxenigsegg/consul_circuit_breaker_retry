namespace CCBR.Services.NotificationService;

public interface IPushNotificationService
{
    Task SendPushNotificationAsync(List<string> deviceTokens, string title, string message);
    Task SendPushNotificationAsync(string deviceToken, string title, string message);
}

public class PushNotificationService(ILogger<PushNotificationService> logger) : IPushNotificationService
{
    private readonly ILogger<PushNotificationService> _logger = logger;

    public async Task SendPushNotificationAsync(List<string> deviceTokens, string title, string message)
    {
        foreach (var token in deviceTokens)
        {
            await SendPushNotificationAsync(token, title, message);
        }
    }

    public async Task SendPushNotificationAsync(string deviceToken, string title, string message)
    {
        // Mock implementation for local development
        _logger.LogInformation("MOCK PUSH: To={DeviceToken}, Title={Title}, Message={Message}",
            deviceToken, title, message);

        // Simulate network delay
        await Task.Delay(100);
    }
}
