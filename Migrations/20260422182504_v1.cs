using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartWaste.Migrations
{
    /// <inheritdoc />
    public partial class v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HubStaff",
                columns: table => new
                {
                    StaffID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    passwordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Sorter")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HubStaff__96D4AAF74529BE54", x => x.StaffID);
                });

            migrationBuilder.CreateTable(
                name: "Recyclers",
                columns: table => new
                {
                    RecyclerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    passwordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleInfo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Offline"),
                    Rating = table.Column<decimal>(type: "decimal(3,2)", nullable: true, defaultValue: 5.0m),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Recycler__CF8A55D5FEB653EF", x => x.RecyclerID);
                });

            migrationBuilder.CreateTable(
                name: "Rewards",
                columns: table => new
                {
                    RewardID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CostPoints = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Rewards__8250159966275F67", x => x.RewardID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WalletPoints = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0.0m),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__1788CCAC6EBB6AB5", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "WasteCategories",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PointsPerUnit = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    UnitType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Item")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__WasteCat__19093A2BB1FAC898", x => x.CategoryID);
                });

            migrationBuilder.CreateTable(
                name: "PickupRequests",
                columns: table => new
                {
                    RequestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    RecyclerID = table.Column<int>(type: "int", nullable: true),
                    HubStaffID = table.Column<int>(type: "int", nullable: true),
                    RequestDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    PickupDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    VerificationDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    QRCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EstimatedPoints = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    FinalPoints = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PickupRe__33A8519AFA03C46A", x => x.RequestID);
                    table.ForeignKey(
                        name: "FK_Requests_Recycler",
                        column: x => x.RecyclerID,
                        principalTable: "Recyclers",
                        principalColumn: "RecyclerID");
                    table.ForeignKey(
                        name: "FK_Requests_Staff",
                        column: x => x.HubStaffID,
                        principalTable: "HubStaff",
                        principalColumn: "StaffID");
                    table.ForeignKey(
                        name: "FK_Requests_User",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "SupportTickets",
                columns: table => new
                {
                    TicketID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CitizenID = table.Column<int>(type: "int", nullable: false),
                    DriverID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTickets", x => x.TicketID);
                    table.ForeignKey(
                        name: "FK_SupportTickets_Recyclers_DriverID",
                        column: x => x.DriverID,
                        principalTable: "Recyclers",
                        principalColumn: "RecyclerID");
                    table.ForeignKey(
                        name: "FK_SupportTickets_Users_CitizenID",
                        column: x => x.CitizenID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRedemptions",
                columns: table => new
                {
                    RedemptionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    RewardID = table.Column<int>(type: "int", nullable: false),
                    RedeemedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    VoucherCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserRede__410680D10B55621C", x => x.RedemptionID);
                    table.ForeignKey(
                        name: "FK_Redemption_Reward",
                        column: x => x.RewardID,
                        principalTable: "Rewards",
                        principalColumn: "RewardID");
                    table.ForeignKey(
                        name: "FK_Redemption_User",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    FeedbackID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestID = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Feedback__6A4BEDF6AA4CCE99", x => x.FeedbackID);
                    table.ForeignKey(
                        name: "FK_Feedback_Request",
                        column: x => x.RequestID,
                        principalTable: "PickupRequests",
                        principalColumn: "RequestID");
                });

            migrationBuilder.CreateTable(
                name: "RequestItems",
                columns: table => new
                {
                    ItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestID = table.Column<int>(type: "int", nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RequestI__727E83EB51E2336E", x => x.ItemID);
                    table.ForeignKey(
                        name: "FK_Items_Category",
                        column: x => x.CategoryID,
                        principalTable: "WasteCategories",
                        principalColumn: "CategoryID");
                    table.ForeignKey(
                        name: "FK_Items_Request",
                        column: x => x.RequestID,
                        principalTable: "PickupRequests",
                        principalColumn: "RequestID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_RequestID",
                table: "Feedbacks",
                column: "RequestID");

            migrationBuilder.CreateIndex(
                name: "IX_PickupRequests_HubStaffID",
                table: "PickupRequests",
                column: "HubStaffID");

            migrationBuilder.CreateIndex(
                name: "IX_PickupRequests_RecyclerID",
                table: "PickupRequests",
                column: "RecyclerID");

            migrationBuilder.CreateIndex(
                name: "IX_PickupRequests_UserID",
                table: "PickupRequests",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "UQ__Recycler__5C7E359E9AF9A312",
                table: "Recyclers",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestItems_CategoryID",
                table: "RequestItems",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_RequestItems_RequestID",
                table: "RequestItems",
                column: "RequestID");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_CitizenID",
                table: "SupportTickets",
                column: "CitizenID");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_DriverID",
                table: "SupportTickets",
                column: "DriverID");

            migrationBuilder.CreateIndex(
                name: "IX_UserRedemptions_RewardID",
                table: "UserRedemptions",
                column: "RewardID");

            migrationBuilder.CreateIndex(
                name: "IX_UserRedemptions_UserID",
                table: "UserRedemptions",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "UQ__Users__A9D10534AFF072C8",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "RequestItems");

            migrationBuilder.DropTable(
                name: "SupportTickets");

            migrationBuilder.DropTable(
                name: "UserRedemptions");

            migrationBuilder.DropTable(
                name: "WasteCategories");

            migrationBuilder.DropTable(
                name: "PickupRequests");

            migrationBuilder.DropTable(
                name: "Rewards");

            migrationBuilder.DropTable(
                name: "Recyclers");

            migrationBuilder.DropTable(
                name: "HubStaff");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
