using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RebooTech.Fruitlogix.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddInspectorAndInvoiceIssuer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "inspector_name",
                table: "quality_inspections",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "inspector_user_id",
                table: "quality_inspections",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "issued_by_user_id",
                table: "invoices",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "inspector_name",
                table: "quality_inspections");

            migrationBuilder.DropColumn(
                name: "inspector_user_id",
                table: "quality_inspections");

            migrationBuilder.DropColumn(
                name: "issued_by_user_id",
                table: "invoices");
        }
    }
}
