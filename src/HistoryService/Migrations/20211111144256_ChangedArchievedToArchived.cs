using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HistoryService.Migrations
{
    public partial class ChangedArchievedToArchived : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchievedOrders");

            migrationBuilder.CreateTable(
                name: "ArchivedOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    SubmitDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Manager = table.Column<string>(type: "text", nullable: true),
                    ConfirmDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeliveredDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivedOrders", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchivedOrders");

            migrationBuilder.CreateTable(
                name: "ArchievedOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfirmDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeliveredDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    Manager = table.Column<string>(type: "text", nullable: true),
                    SubmitDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchievedOrders", x => x.Id);
                });
        }
    }
}
