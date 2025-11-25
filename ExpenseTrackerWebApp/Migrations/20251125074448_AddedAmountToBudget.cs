using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTrackerWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddedAmountToBudget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Budgets",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Budgets",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Budgets");
        }
    }
}
