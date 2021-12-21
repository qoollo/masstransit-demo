using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrderOrchestratorService.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderStates",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentState = table.Column<int>(type: "integer", nullable: false),
                    SubmitDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    TotalPrice = table.Column<int>(type: "integer", nullable: false),
                    Manager = table.Column<string>(type: "text", nullable: true),
                    IsConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    ConfirmationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeliveryDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStates", x => x.CorrelationId);
                });

            migrationBuilder.CreateTable(
                name: "CartPositions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartPositions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartPositions_OrderStates_OrderId",
                        column: x => x.OrderId,
                        principalTable: "OrderStates",
                        principalColumn: "CorrelationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartPositions_Id",
                table: "CartPositions",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartPositions_OrderId",
                table: "CartPositions",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStates_CorrelationId",
                table: "OrderStates",
                column: "CorrelationId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartPositions");

            migrationBuilder.DropTable(
                name: "OrderStates");
        }
    }
}
