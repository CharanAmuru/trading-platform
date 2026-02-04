using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradingPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialTrading : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Orders_AccountId_CreatedAtUtc",
                table: "Orders",
                columns: new[] { "AccountId", "CreatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_Status_UpdatedAtUtc",
                table: "Orders",
                columns: new[] { "Status", "UpdatedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ExecutionJobs_CreatedAtUtc",
                table: "ExecutionJobs",
                column: "CreatedAtUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_AccountId_CreatedAtUtc",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_Status_UpdatedAtUtc",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_ExecutionJobs_CreatedAtUtc",
                table: "ExecutionJobs");
        }
    }
}
