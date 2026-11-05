using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusPlacementManagement.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureStudentModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CGPA",
                table: "Students",
                type: "decimal(4,2)",
                precision: 4,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CGPA",
                table: "Students",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(4,2)",
                oldPrecision: 4,
                oldScale: 2);
        }
    }
}
