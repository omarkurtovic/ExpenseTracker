using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTrackerWebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddedReferenceToUsersFromLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SystemLogs");

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "SystemLogs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SystemLogs_IdentityUserId",
                table: "SystemLogs",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SystemLogs_AspNetUsers_IdentityUserId",
                table: "SystemLogs",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SystemLogs_AspNetUsers_IdentityUserId",
                table: "SystemLogs");

            migrationBuilder.DropIndex(
                name: "IX_SystemLogs_IdentityUserId",
                table: "SystemLogs");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "SystemLogs");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "SystemLogs",
                type: "TEXT",
                nullable: true);
        }
    }
}
