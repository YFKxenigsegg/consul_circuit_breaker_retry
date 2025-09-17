using Microsoft.EntityFrameworkCore;

namespace CCBR.Services.NotificationService;

public class NotificationDbContext(DbContextOptions<NotificationDbContext> options) : DbContext(options)
{
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationTemplate> NotificationTemplates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Message)
                .IsRequired()
                .HasMaxLength(2000);
            entity.Property(e => e.Channel)
                .IsRequired()
                .HasConversion<string>();
            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasDefaultValue(NotificationStatus.Pending);
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false);
            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");
            entity.Property(e => e.Metadata)
                .HasColumnType("text");
            entity.Property(e => e.RelatedEntityType)
                .HasMaxLength(50);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => new { e.UserId, e.IsRead });
        });

        // NotificationTemplate configuration
        modelBuilder.Entity<NotificationTemplate>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Channel)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(e => e.Subject)
                .HasMaxLength(200);

            entity.Property(e => e.Template)
                .IsRequired()
                .HasColumnType("text");

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            entity.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("NOW()");

            // Indexes
            entity.HasIndex(e => new { e.Type, e.Channel }).IsUnique();
        });

        // Seed templates
        SeedNotificationTemplates(modelBuilder);
    }

    private void SeedNotificationTemplates(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotificationTemplate>().HasData(
            // Email templates
            new NotificationTemplate
            {
                Id = 1,
                Type = "OrderCreated",
                Channel = NotificationChannel.Email,
                Subject = "Order Confirmation - {{OrderNumber}}",
                Template = @"
                    <h2>Thank you for your order!</h2>
                    <p>Your order {{OrderNumber}} has been successfully placed.</p>
                    <p><strong>Order Details:</strong></p>
                    <ul>
                        {{#each Items}}
                        <li>{{ProductName}} - Quantity: {{Quantity}} - Price: ${{Price}}</li>
                        {{/each}}
                    </ul>
                    <p><strong>Total: ${{TotalAmount}}</strong></p>
                    <p>Order Date: {{OrderDate}}</p>
                ",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new NotificationTemplate
            {
                Id = 2,
                Type = "OrderShipped",
                Channel = NotificationChannel.Email,
                Subject = "Your Order {{OrderNumber}} Has Been Shipped",
                Template = @"
                    <h2>Your order is on its way!</h2>
                    <p>Your order {{OrderNumber}} has been shipped and is on its way to you.</p>
                    <p>You can track your package using the tracking information provided.</p>
                    <p>Expected delivery: 3-5 business days</p>
                ",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            // In-App templates
            new NotificationTemplate
            {
                Id = 4,
                Type = "OrderCreated",
                Channel = NotificationChannel.InApp,
                Subject = "Order Confirmed",
                Template = "Your order {{OrderNumber}} for ${{TotalAmount}} has been confirmed and is being processed.",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
