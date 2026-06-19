using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartWaste.Migrations
{
    /// <inheritdoc />
    public partial class v4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropIndex(
            //    name: "UQ__Recycler__5C7E359E9AF9A312",
            //    table: "Recyclers");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Recyclers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.CreateIndex(
                name: "UQ__Recycler__5C7E359E9AF9A312",
                table: "Recyclers",
                column: "Phone",
                unique: true,
                filter: "[Phone] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UQ__Recycler__5C7E359E9AF9A312",
                table: "Recyclers");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Recyclers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Recycler__5C7E359E9AF9A312",
                table: "Recyclers",
                column: "Phone",
                unique: true);
        }
    }
}
