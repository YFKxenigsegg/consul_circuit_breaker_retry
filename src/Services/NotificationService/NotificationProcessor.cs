namespace CCBR.Services.NotificationService;

public class NotificationProcessor(
    IServiceProvider serviceProvider,
    ILogger<NotificationProcessor> logger)
    : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ILogger<NotificationProcessor> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
                await notificationService.ProcessPendingNotificationsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing pending notifications");
            }
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
