using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartWaste.Migrations
{
    /// <inheritdoc />
    public partial class v9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentCapacity",
                table: "Recyclers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CurrentLatitude",
                table: "Recyclers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CurrentLongitude",
                table: "Recyclers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentStatus",
                table: "Recyclers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentCapacity",
                table: "Recyclers");

            migrationBuilder.DropColumn(
                name: "CurrentLatitude",
                table: "Recyclers");

            migrationBuilder.DropColumn(
                name: "CurrentLongitude",
                table: "Recyclers");

            migrationBuilder.DropColumn(
                name: "CurrentStatus",
                table: "Recyclers");
        }
    }
}
