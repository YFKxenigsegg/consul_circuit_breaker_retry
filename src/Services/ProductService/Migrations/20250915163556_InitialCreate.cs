using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProductService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Stock = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "IsActive", "Name", "Price", "Stock" },
                values: new object[,]
                {
                    { 1, "Electronics", new DateTime(2025, 9, 15, 16, 35, 51, 710, DateTimeKind.Utc).AddTicks(4241), "High-performance gaming laptop with RTX graphics", true, "Gaming Laptop", 1299.99m, 25 },
                    { 2, "Electronics", new DateTime(2025, 9, 15, 16, 35, 51, 710, DateTimeKind.Utc).AddTicks(4608), "Ergonomic wireless mouse for gaming and productivity", true, "Wireless Mouse", 49.99m, 150 },
                    { 3, "Furniture", new DateTime(2025, 9, 15, 16, 35, 51, 710, DateTimeKind.Utc).AddTicks(4612), "Comfortable ergonomic office chair", true, "Office Chair", 299.99m, 30 },
                    { 4, "Books", new DateTime(2025, 9, 15, 16, 35, 51, 710, DateTimeKind.Utc).AddTicks(4615), "Learn C# programming from basics to advanced", true, "Programming Book", 39.99m, 100 },
                    { 5, "Home", new DateTime(2025, 9, 15, 16, 35, 51, 710, DateTimeKind.Utc).AddTicks(4618), "Premium ceramic coffee mug", true, "Coffee Mug", 14.99m, 200 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Category",
                table: "Products",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Category_IsActive",
                table: "Products",
                columns: new[] { "Category", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsActive",
                table: "Products",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name",
                table: "Products",
                column: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
