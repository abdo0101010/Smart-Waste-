using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartWaste.Migrations
{
    /// <inheritdoc />
    public partial class v8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RecyclerId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RecyclerId",
                table: "Notifications",
                column: "RecyclerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Recyclers_RecyclerId",
                table: "Notifications",
                column: "RecyclerId",
                principalTable: "Recyclers",
                principalColumn: "RecyclerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Recyclers_RecyclerId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_RecyclerId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "RecyclerId",
                table: "Notifications");
        }
    }
}
