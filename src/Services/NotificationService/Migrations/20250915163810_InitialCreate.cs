using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NotificationService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Channel = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "Pending"),
                    IsRead = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    SentAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReadAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Metadata = table.Column<string>(type: "text", nullable: true),
                    RelatedEntityId = table.Column<int>(type: "integer", nullable: true),
                    RelatedEntityType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Channel = table.Column<string>(type: "text", nullable: false),
                    Subject = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Template = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTemplates", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "NotificationTemplates",
                columns: new[] { "Id", "Channel", "CreatedAt", "IsActive", "Subject", "Template", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Email", new DateTime(2025, 9, 15, 16, 38, 4, 675, DateTimeKind.Utc).AddTicks(4016), true, "Order Confirmation - {{OrderNumber}}", "\r\n                    <h2>Thank you for your order!</h2>\r\n                    <p>Your order {{OrderNumber}} has been successfully placed.</p>\r\n                    <p><strong>Order Details:</strong></p>\r\n                    <ul>\r\n                        {{#each Items}}\r\n                        <li>{{ProductName}} - Quantity: {{Quantity}} - Price: ${{Price}}</li>\r\n                        {{/each}}\r\n                    </ul>\r\n                    <p><strong>Total: ${{TotalAmount}}</strong></p>\r\n                    <p>Order Date: {{OrderDate}}</p>\r\n                ", "OrderCreated", new DateTime(2025, 9, 15, 16, 38, 4, 675, DateTimeKind.Utc).AddTicks(4415) },
                    { 2, "Email", new DateTime(2025, 9, 15, 16, 38, 4, 675, DateTimeKind.Utc).AddTicks(4636), true, "Your Order {{OrderNumber}} Has Been Shipped", "\r\n                    <h2>Your order is on its way!</h2>\r\n                    <p>Your order {{OrderNumber}} has been shipped and is on its way to you.</p>\r\n                    <p>You can track your package using the tracking information provided.</p>\r\n                    <p>Expected delivery: 3-5 business days</p>\r\n                ", "OrderShipped", new DateTime(2025, 9, 15, 16, 38, 4, 675, DateTimeKind.Utc).AddTicks(4637) },
                    { 4, "InApp", new DateTime(2025, 9, 15, 16, 38, 4, 675, DateTimeKind.Utc).AddTicks(4640), true, "Order Confirmed", "Your order {{OrderNumber}} for ${{TotalAmount}} has been confirmed and is being processed.", "OrderCreated", new DateTime(2025, 9, 15, 16, 38, 4, 675, DateTimeKind.Utc).AddTicks(4641) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreatedAt",
                table: "Notifications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Status",
                table: "Notifications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Type",
                table: "Notifications",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_IsRead",
                table: "Notifications",
                columns: new[] { "UserId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationTemplates_Type_Channel",
                table: "NotificationTemplates",
                columns: new[] { "Type", "Channel" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "NotificationTemplates");
        }
    }
}
