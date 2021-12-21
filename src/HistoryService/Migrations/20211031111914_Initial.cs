using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HistoryService.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArchievedOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalPrice = table.Column<int>(type: "integer", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    SubmitDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Manager = table.Column<string>(type: "text", nullable: true),
                    ConfirmDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeliveredDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchievedOrders", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchievedOrders");
        }
    }
}
