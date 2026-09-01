using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SNS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCompanyCreateRequestReviewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedCompanyId",
                schema: "Jobs",
                table: "CompanyCreateRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewNote",
                schema: "Jobs",
                table: "CompanyCreateRequests",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReviewedByProfileId",
                schema: "Jobs",
                table: "CompanyCreateRequests",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedCompanyId",
                schema: "Jobs",
                table: "CompanyCreateRequests");

            migrationBuilder.DropColumn(
                name: "ReviewNote",
                schema: "Jobs",
                table: "CompanyCreateRequests");

            migrationBuilder.DropColumn(
                name: "ReviewedByProfileId",
                schema: "Jobs",
                table: "CompanyCreateRequests");
        }
    }
}
