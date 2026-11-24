using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace RebooTech.Fruitlogix.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveryEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "arrived_at",
                table: "deliveries",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "delivered_at",
                table: "deliveries",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "dispatched_at",
                table: "deliveries",
                type: "datetime",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "delivery_events",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    delivery_id = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    note = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    latitude = table.Column<double>(type: "double", nullable: true),
                    longitude = table.Column<double>(type: "double", nullable: true),
                    occurred_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    created_by_user_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_delivery_events", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_delivery_events_delivery_id",
                table: "delivery_events",
                column: "delivery_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "delivery_events");

            migrationBuilder.DropColumn(
                name: "arrived_at",
                table: "deliveries");

            migrationBuilder.DropColumn(
                name: "delivered_at",
                table: "deliveries");

            migrationBuilder.DropColumn(
                name: "dispatched_at",
                table: "deliveries");
        }
    }
}
