using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SNS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropCommentMediaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentMedias",
                schema: "ContentManagement");

            migrationBuilder.EnsureSchema(
                name: "Moderation");

            migrationBuilder.EnsureSchema(
                name: "Support");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "Projects",
                table: "ProjectMedias",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "ReportTickets",
                schema: "Moderation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetType = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReportCount = table.Column<int>(type: "int", nullable: false),
                    ModeratorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModeratorNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportTickets_Users_ModeratorId",
                        column: x => x.ModeratorId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SupportTickets",
                schema: "Support",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAgentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTickets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentReports",
                schema: "Moderation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReporterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ViolationReason = table.Column<int>(type: "int", nullable: false),
                    AdditionalDetails = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentReports_ReportTickets_TicketId",
                        column: x => x.TicketId,
                        principalSchema: "Moderation",
                        principalTable: "ReportTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketMessages",
                schema: "Support",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsFromAgent = table.Column<bool>(type: "bit", nullable: false),
                    MessageBody = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    AttachmentUrl = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketMessages_SupportTickets_TicketId",
                        column: x => x.TicketId,
                        principalSchema: "Support",
                        principalTable: "SupportTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentReports_ReporterId",
                schema: "Moderation",
                table: "ContentReports",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentReports_TicketId",
                schema: "Moderation",
                table: "ContentReports",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTickets_ModeratorId",
                schema: "Moderation",
                table: "ReportTickets",
                column: "ModeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportTickets_TargetId",
                schema: "Moderation",
                table: "ReportTickets",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_AssignedAgentId",
                schema: "Support",
                table: "SupportTickets",
                column: "AssignedAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_UserId",
                schema: "Support",
                table: "SupportTickets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketMessages_SenderId",
                schema: "Support",
                table: "TicketMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketMessages_TicketId",
                schema: "Support",
                table: "TicketMessages",
                column: "TicketId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentReports",
                schema: "Moderation");

            migrationBuilder.DropTable(
                name: "TicketMessages",
                schema: "Support");

            migrationBuilder.DropTable(
                name: "ReportTickets",
                schema: "Moderation");

            migrationBuilder.DropTable(
                name: "SupportTickets",
                schema: "Support");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "Projects",
                table: "ProjectMedias");

            migrationBuilder.CreateTable(
                name: "CommentMedias",
                schema: "ContentManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Duration = table.Column<double>(type: "float", nullable: true),
                    Height = table.Column<int>(type: "int", nullable: true),
                    MimeType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false),
                    Width = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentMedias_Comments_CommentId",
                        column: x => x.CommentId,
                        principalSchema: "ContentManagement",
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentMedias_CommentId",
                schema: "ContentManagement",
                table: "CommentMedias",
                column: "CommentId");
        }
    }
}
