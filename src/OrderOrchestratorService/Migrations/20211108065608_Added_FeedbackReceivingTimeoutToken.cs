using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderOrchestratorService.Migrations
{
    public partial class Added_FeedbackReceivingTimeoutToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FeedbackReceivingTimeoutToken",
                table: "OrderStates",
                type: "uuid",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeedbackReceivingTimeoutToken",
                table: "OrderStates");
        }
    }
}
