using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XakUjin2025.Migrations
{
    /// <inheritdoc />
    public partial class FixIndicationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Signals",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Apartments",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Signals");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Apartments");
        }
    }
}
