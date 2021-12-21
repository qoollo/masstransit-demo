using Microsoft.EntityFrameworkCore.Migrations;

namespace HistoryService.Migrations
{
    public partial class RemovedTotalPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "ArchievedOrders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalPrice",
                table: "ArchievedOrders",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
