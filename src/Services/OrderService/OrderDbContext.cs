using Microsoft.EntityFrameworkCore;

namespace CCBR.Services.OrderService;

public class OrderDbContext(DbContextOptions<OrderDbContext> options)
    : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();
            entity.Property(e => e.UserId)
                .IsRequired();
            entity.Property(e => e.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>();
            entity.Property(e => e.CreatedAt)
                .IsRequired();
            entity.HasMany(e => e.Items)
                .WithOne()
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();
            entity.Property(e => e.OrderId)
                .IsRequired();
            entity.Property(e => e.ProductId)
                .IsRequired();
            entity.Property(e => e.Quantity)
                .IsRequired();
            entity.Property(e => e.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<Order>().HasIndex(e => e.UserId);
        modelBuilder.Entity<Order>().HasIndex(e => e.Status);
        modelBuilder.Entity<OrderItem>().HasIndex(e => e.OrderId);
        modelBuilder.Entity<OrderItem>().HasIndex(e => e.ProductId);
    }
}