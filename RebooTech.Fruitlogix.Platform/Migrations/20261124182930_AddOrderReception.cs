using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RebooTech.Fruitlogix.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderReception : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "delivered_at",
                table: "orders",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reception_comment",
                table: "orders",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "reception_rating",
                table: "orders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "seal_and_temperature_ok",
                table: "orders",
                type: "tinyint(1)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "delivered_at",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "reception_comment",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "reception_rating",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "seal_and_temperature_ok",
                table: "orders");
        }
    }
}
