using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SNS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterDeviceTokenConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Devices_DeviceToken",
                schema: "Identity",
                table: "Devices");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DeviceToken_UserId",
                schema: "Identity",
                table: "Devices",
                columns: new[] { "DeviceToken", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Devices_DeviceToken_UserId",
                schema: "Identity",
                table: "Devices");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DeviceToken",
                schema: "Identity",
                table: "Devices",
                column: "DeviceToken",
                unique: true);
        }
    }
}
