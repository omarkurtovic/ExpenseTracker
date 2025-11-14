using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Migrations
{
    /// <inheritdoc />
    public partial class DefaultUserInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1b1c59f2-891f-4732-a974-3b755208d0d9", "a95a997e-84dd-4ef6-a759-1f36700a41f4", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "4e08d54b-16f0-47a0-afaf-afc12dbdedc8", 0, "a95a997e-84dd-4ef6-a759-1f36700a41f4", null, false, false, null, null, "SA", "AQAAAAIAAYagAAAAEPaPOWihwWXqakYt44g4+tcyL/Re1e5Fx3AiCOMXavq7m9bkrUdn20iJc8ABi9u72A==", null, false, "c3691bab-bed8-4bcc-91fa-62bb12e2b245", false, "sa" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1b1c59f2-891f-4732-a974-3b755208d0d9", "4e08d54b-16f0-47a0-afaf-afc12dbdedc8" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1b1c59f2-891f-4732-a974-3b755208d0d9", "4e08d54b-16f0-47a0-afaf-afc12dbdedc8" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1b1c59f2-891f-4732-a974-3b755208d0d9");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4e08d54b-16f0-47a0-afaf-afc12dbdedc8");
        }
    }
}
