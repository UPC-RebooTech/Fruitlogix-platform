using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace RebooTech.Fruitlogix.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddFleetAndHarvestPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "driver_id",
                table: "deliveries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "vehicle_id",
                table: "deliveries",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "drivers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    phone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true),
                    license_class = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    license_expiry = table.Column<DateTime>(type: "date", nullable: false),
                    cold_chain_certified = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    rest_until = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    created_by_user_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_drivers", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "harvest_plans",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    producer_id = table.Column<int>(type: "int", nullable: false),
                    fruit_type = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    scheduled_for = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    estimated_yield_kg = table.Column<double>(type: "double", nullable: false),
                    zone = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    crew_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    pickers = table.Column<int>(type: "int", nullable: false),
                    notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_harvest_plans", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vehicles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    unit_code = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    plate = table.Column<string>(type: "varchar(7)", maxLength: 7, nullable: false),
                    type = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    capacity_kg = table.Column<double>(type: "double", nullable: false),
                    min_celsius = table.Column<double>(type: "double", nullable: false),
                    max_celsius = table.Column<double>(type: "double", nullable: false),
                    status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    created_by_user_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_vehicles", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_deliveries_driver_id",
                table: "deliveries",
                column: "driver_id");

            migrationBuilder.CreateIndex(
                name: "i_x_deliveries_vehicle_id",
                table: "deliveries",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "i_x_harvest_plans_producer_id",
                table: "harvest_plans",
                column: "producer_id");

            migrationBuilder.CreateIndex(
                name: "i_x_vehicles_plate",
                table: "vehicles",
                column: "plate",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "drivers");

            migrationBuilder.DropTable(
                name: "harvest_plans");

            migrationBuilder.DropTable(
                name: "vehicles");

            migrationBuilder.DropIndex(
                name: "i_x_deliveries_driver_id",
                table: "deliveries");

            migrationBuilder.DropIndex(
                name: "i_x_deliveries_vehicle_id",
                table: "deliveries");

            migrationBuilder.DropColumn(
                name: "driver_id",
                table: "deliveries");

            migrationBuilder.DropColumn(
                name: "vehicle_id",
                table: "deliveries");
        }
    }
}
