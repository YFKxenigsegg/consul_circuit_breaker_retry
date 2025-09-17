using Microsoft.EntityFrameworkCore;

namespace CCBR.Services.ProductService;

public class ProductDbContext(DbContextOptions<ProductDbContext> options)
    : DbContext(options)
{
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .HasMaxLength(2000);

            entity.Property(e => e.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.Stock)
                .IsRequired()
                .HasDefaultValue(0);

            entity.Property(e => e.Category)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);
        });

        // Create indexes for better performance
        modelBuilder.Entity<Product>()
            .HasIndex(e => e.Category);

        modelBuilder.Entity<Product>()
            .HasIndex(e => e.IsActive);

        modelBuilder.Entity<Product>()
            .HasIndex(e => e.Name);

        modelBuilder.Entity<Product>()
            .HasIndex(e => new { e.Category, e.IsActive });

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Gaming Laptop",
                Description = "High-performance gaming laptop with RTX graphics",
                Price = 1299.99m,
                Stock = 25,
                Category = "Electronics",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Product
            {
                Id = 2,
                Name = "Wireless Mouse",
                Description = "Ergonomic wireless mouse for gaming and productivity",
                Price = 49.99m,
                Stock = 150,
                Category = "Electronics",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Product
            {
                Id = 3,
                Name = "Office Chair",
                Description = "Comfortable ergonomic office chair",
                Price = 299.99m,
                Stock = 30,
                Category = "Furniture",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Product
            {
                Id = 4,
                Name = "Programming Book",
                Description = "Learn C# programming from basics to advanced",
                Price = 39.99m,
                Stock = 100,
                Category = "Books",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            },
            new Product
            {
                Id = 5,
                Name = "Coffee Mug",
                Description = "Premium ceramic coffee mug",
                Price = 14.99m,
                Stock = 200,
                Category = "Home",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true
            }
        );
    }
}
