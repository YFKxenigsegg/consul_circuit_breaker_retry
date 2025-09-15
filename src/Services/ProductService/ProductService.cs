using Microsoft.EntityFrameworkCore;

namespace CCBR.Services.ProductService;

public interface IProductService
{
    Task<Product> GetProductAsync(int id);
    Task<IEnumerable<Product>> GetProductsAsync();
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
    Task<Product> CreateProductAsync(Product product);
    Task<bool> UpdateStockAsync(int productId, int quantity);
    Task<bool> IsProductAvailableAsync(int productId, int quantity);
}

public class ProductService(
    ProductDbContext context,
    ILogger<ProductService> logger)
    : IProductService
{
    private readonly ProductDbContext _context = context;
    private readonly ILogger<ProductService> _logger = logger;

    public async Task<Product> GetProductAsync(int id) =>
        await _context.Products.FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        _logger.LogInformation("Getting all products");
        return await _context.Products.Where(p => p.IsActive).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
    {
        _logger.LogInformation($"Getting products by category: {category}");
        return await _context.Products
            .Where(p => p.Category == category && p.IsActive)
            .ToListAsync();
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        _logger.LogInformation($"Creating product: {product.Name}");

        product.CreatedAt = DateTime.UtcNow;
        product.IsActive = true;

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return product;
    }

    public async Task<bool> UpdateStockAsync(int productId, int quantity)
    {
        _logger.LogInformation($"Updating stock for product {productId}, quantity: {quantity}");

        var product = await _context.Products.FindAsync(productId);
        if (product == null || product.Stock + quantity < 0)
            return false;

        product.Stock += quantity;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> IsProductAvailableAsync(int productId, int quantity)
    {
        var product = await _context.Products.FindAsync(productId);
        return product != null && product.IsActive && product.Stock >= quantity;
    }
}
