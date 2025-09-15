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
    ILogger<OrderService> logger) : IOrderService
{
    private readonly OrderDbContext _context = context;
    private readonly ILogger<OrderService> _logger = logger;

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        _logger.LogInformation($"Creating order for user: {request.UserId}");

        // Note: In a proper microservices architecture, validation of user and products
        // should be handled by the API Gateway or through separate validation calls.
        // For this simplified example, we'll create the order directly.
        
        decimal totalAmount = 0;
        var orderItems = new List<OrderItem>();

        foreach (var item in request.Items)
        {
            // In a real implementation, product validation and pricing would be done
            // through the API Gateway or by calling the Product Service via the gateway
            var orderItem = new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = 0 // This would be fetched from Product Service via API Gateway
            };

            orderItems.Add(orderItem);
            totalAmount += orderItem.Price * orderItem.Quantity;
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

        // Note: Notifications would typically be sent via the API Gateway
        // or through an event-driven architecture (message queues, event bus, etc.)
        _logger.LogInformation($"Order {order.Id} created successfully for user {request.UserId}");

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
