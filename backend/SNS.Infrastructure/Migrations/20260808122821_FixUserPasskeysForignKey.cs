using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SNS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixUserPasskeysForignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPassKeys_Users_Id",
                schema: "Identity",
                table: "UserPassKeys");

            migrationBuilder.CreateIndex(
                name: "IX_UserPassKeys_UserId",
                schema: "Identity",
                table: "UserPassKeys",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPassKeys_Users_UserId",
                schema: "Identity",
                table: "UserPassKeys",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPassKeys_Users_UserId",
                schema: "Identity",
                table: "UserPassKeys");

            migrationBuilder.DropIndex(
                name: "IX_UserPassKeys_UserId",
                schema: "Identity",
                table: "UserPassKeys");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPassKeys_Users_Id",
                schema: "Identity",
                table: "UserPassKeys",
                column: "Id",
                principalSchema: "Identity",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
