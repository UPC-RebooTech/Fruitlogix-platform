using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RebooTech.Fruitlogix.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddRolesAndOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "role",
                table: "users",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "CommercialClient");

            migrationBuilder.AddColumn<int>(
                name: "user_id",
                table: "producers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "order_id",
                table: "incidents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "reported_by_user_id",
                table: "incidents",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "distributor_user_id",
                table: "deliveries",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "i_x_producers_user_id",
                table: "producers",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "i_x_incidents_order_id",
                table: "incidents",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "i_x_incidents_reported_by_user_id",
                table: "incidents",
                column: "reported_by_user_id");

            migrationBuilder.CreateIndex(
                name: "i_x_deliveries_distributor_user_id",
                table: "deliveries",
                column: "distributor_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_producers_user_id",
                table: "producers");

            migrationBuilder.DropIndex(
                name: "i_x_incidents_order_id",
                table: "incidents");

            migrationBuilder.DropIndex(
                name: "i_x_incidents_reported_by_user_id",
                table: "incidents");

            migrationBuilder.DropIndex(
                name: "i_x_deliveries_distributor_user_id",
                table: "deliveries");

            migrationBuilder.DropColumn(
                name: "role",
                table: "users");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "producers");

            migrationBuilder.DropColumn(
                name: "order_id",
                table: "incidents");

            migrationBuilder.DropColumn(
                name: "reported_by_user_id",
                table: "incidents");

            migrationBuilder.DropColumn(
                name: "distributor_user_id",
                table: "deliveries");
        }
    }
}
