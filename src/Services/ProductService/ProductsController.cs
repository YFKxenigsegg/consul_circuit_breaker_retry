using Microsoft.AspNetCore.Mvc;

namespace CCBR.Services.ProductService;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        var products = await _productService.GetProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _productService.GetProductAsync(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpGet("category/{category}")]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(string category)
    {
        var products = await _productService.GetProductsByCategoryAsync(category);
        return Ok(products);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        try
        {
            var createdProduct = await _productService.CreateProductAsync(product);
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}/stock")]
    public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockRequest request)
    {
        var result = await _productService.UpdateStockAsync(id, request.Quantity);
        if (!result) return BadRequest("Unable to update stock");
        return Ok();
    }

    [HttpGet("{id}/availability")]
    public async Task<ActionResult<bool>> CheckAvailability(int id, [FromQuery] int quantity)
    {
        var isAvailable = await _productService.IsProductAvailableAsync(id, quantity);
        return Ok(isAvailable);
    }
}

public class UpdateStockRequest
{
    public int Quantity { get; set; }
}
