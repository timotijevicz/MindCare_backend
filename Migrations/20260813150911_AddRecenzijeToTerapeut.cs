using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MentalHealthApi.Migrations
{
    /// <inheritdoc />
    public partial class AddRecenzijeToTerapeut : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BrojRecenzija",
                table: "Terapeuti",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "ProsecnaOcena",
                table: "Terapeuti",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrojRecenzija",
                table: "Terapeuti");

            migrationBuilder.DropColumn(
                name: "ProsecnaOcena",
                table: "Terapeuti");
        }
    }
}
