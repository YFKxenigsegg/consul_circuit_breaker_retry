using CCBR.Shared.ServiceDiscovery;
using CCBR.Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace CCBR.Services.OrderService;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(CreateOrderRequest request);
    Task<Order> GetOrderAsync(int id);
    Task<IEnumerable<Order>> GetUserOrdersAsync(int userId);
    Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status);
}

public class OrderService(
    OrderDbContext context,
    ResilientHttpClient httpClient,
    IServiceDiscovery serviceDiscovery,
    ILogger<OrderService> logger) : IOrderService
{
    private readonly OrderDbContext _context = context;
    private readonly ResilientHttpClient _httpClient = httpClient;
    private readonly IServiceDiscovery _serviceDiscovery = serviceDiscovery;
    private readonly ILogger<OrderService> _logger = logger;

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        _logger.LogInformation($"Creating order for user: {request.UserId}");

        // Validate user exists
        var userServiceUrl = await _serviceDiscovery.GetServiceUrlAsync("user-service");
        var userResponse = await _httpClient.GetAsync($"{userServiceUrl}/api/users/{request.UserId}");

        if (!userResponse.IsSuccessStatusCode)
        {
            throw new InvalidOperationException("User not found");
        }

        // Validate products and calculate total
        var productServiceUrl = await _serviceDiscovery.GetServiceUrlAsync("product-service");
        decimal totalAmount = 0;
        var orderItems = new List<OrderItem>();

        foreach (var item in request.Items)
        {
            var productResponse = await _httpClient.GetAsync<Product>(
                $"{productServiceUrl}/api/products/{item.ProductId}");

            // Check stock availability
            var stockResponse = await _httpClient.GetAsync(
                $"{productServiceUrl}/api/products/{item.ProductId}/availability?quantity={item.Quantity}");

            if (!stockResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Product {item.ProductId} not available in requested quantity");
            }

            var orderItem = new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = productResponse.Price
            };

            orderItems.Add(orderItem);
            totalAmount += orderItem.Price * orderItem.Quantity;

            // Reserve stock
            await _httpClient.PostAsync(
                $"{productServiceUrl}/api/products/{item.ProductId}/reserve",
                JsonContent.Create(new { item.Quantity }));
        }

        var order = new Order
        {
            UserId = request.UserId,
            TotalAmount = totalAmount,
            Status = OrderStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            Items = orderItems
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        try
        {
            var notificationServiceUrl = await _serviceDiscovery.GetServiceUrlAsync("notification-service");
            await _httpClient.PostAsync(
                $"{notificationServiceUrl}/api/notifications",
                JsonContent.Create(new
                {
                    request.UserId,
                    Type = "OrderCreated",
                    Message = $"Order #{order.Id} has been created successfully"
                }));
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Failed to send notification: {ex.Message}");
            // Don't fail the order creation if notification fails
        }

        return order;
    }

    public async Task<Order> GetOrderAsync(int id) =>
        await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id);

    public async Task<IEnumerable<Order>> GetUserOrdersAsync(int userId) =>
        await _context.Orders
            .Include(o => o.Items)
            .Where(o => o.UserId == userId)
            .ToListAsync();

    public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null)
            return false;

        order.Status = status;
        await _context.SaveChangesAsync();

        return true;
    }
}
