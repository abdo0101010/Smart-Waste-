using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartWaste.Migrations
{
    /// <inheritdoc />
    public partial class AddEcoSnapImagesToRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FinalBottlesCount",
                table: "RequestItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestImageUrl",
                table: "RequestItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationImageUrl",
                table: "RequestItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FinalBottlesCount",
                table: "PickupRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestImageUrl",
                table: "PickupRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationImageUrl",
                table: "PickupRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalBottlesCount",
                table: "RequestItems");

            migrationBuilder.DropColumn(
                name: "RequestImageUrl",
                table: "RequestItems");

            migrationBuilder.DropColumn(
                name: "VerificationImageUrl",
                table: "RequestItems");

            migrationBuilder.DropColumn(
                name: "FinalBottlesCount",
                table: "PickupRequests");

            migrationBuilder.DropColumn(
                name: "RequestImageUrl",
                table: "PickupRequests");

            migrationBuilder.DropColumn(
                name: "VerificationImageUrl",
                table: "PickupRequests");
        }
    }
}
