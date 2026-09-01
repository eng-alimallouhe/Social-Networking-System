using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SNS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTicketMessageAttachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentUrl",
                schema: "Support",
                table: "TicketMessages");

            migrationBuilder.CreateTable(
                name: "TicketMessageAttachments",
                schema: "Support",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ObjectKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketMessageAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketMessageAttachments_TicketMessages_TicketMessageId",
                        column: x => x.TicketMessageId,
                        principalSchema: "Support",
                        principalTable: "TicketMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketMessageAttachments_TicketMessageId",
                schema: "Support",
                table: "TicketMessageAttachments",
                column: "TicketMessageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketMessageAttachments",
                schema: "Support");

            migrationBuilder.AddColumn<string>(
                name: "AttachmentUrl",
                schema: "Support",
                table: "TicketMessages",
                type: "varchar(512)",
                maxLength: 512,
                nullable: true);
        }
    }
}
