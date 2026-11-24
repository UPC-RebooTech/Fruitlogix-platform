using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RebooTech.Fruitlogix.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderBatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "batch_id",
                table: "orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "i_x_orders_batch_id",
                table: "orders",
                column: "batch_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_orders_batch_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "batch_id",
                table: "orders");
        }
    }
}
