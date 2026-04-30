using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartWaste.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "passwordHash",
                table: "Recyclers",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "passwordHash",
                table: "HubStaff",
                newName: "PasswordHash");

            migrationBuilder.AlterColumn<string>(
                name: "UnitType",
                table: "WasteCategories",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Item",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldDefaultValue: "Item");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureUrl",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePictureUrl",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Recyclers",
                newName: "passwordHash");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "HubStaff",
                newName: "passwordHash");

            migrationBuilder.AlterColumn<string>(
                name: "UnitType",
                table: "WasteCategories",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                defaultValue: "Item",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldDefaultValue: "Item");
        }
    }
}
