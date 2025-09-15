namespace CCBR.Services.NotificationService;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task SendEmailAsync(SendEmailRequest request);
}

public class MockEmailService(ILogger<MockEmailService> logger) : IEmailService
{
    private readonly ILogger<MockEmailService> _logger = logger;

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        var request = new SendEmailRequest
        {
            To = to,
            Subject = subject,
            Body = body,
            IsHtml = isHtml
        };
        await SendEmailAsync(request);
    }

    public async Task SendEmailAsync(SendEmailRequest request)
    {
        // Mock implementation - just log the email details
        _logger.LogInformation("MOCK EMAIL SENT:");
        _logger.LogInformation("To: {To}", request.To);
        _logger.LogInformation("Subject: {Subject}", request.Subject);
        _logger.LogInformation("IsHtml: {IsHtml}", request.IsHtml);
        _logger.LogInformation("Body: {Body}", request.Body);

        if (request.Attachments?.Any() == true)
        {
            _logger.LogInformation("Attachments: {Attachments}", string.Join(", ", request.Attachments));
        }

        // Simulate network delay
        await Task.Delay(200);

        _logger.LogInformation("Mock email sent successfully to {To}", request.To);
    }
}