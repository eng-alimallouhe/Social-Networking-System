using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SNS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateViewDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostViews_Profiles_ViewerId",
                schema: "ContentManagement",
                table: "PostViews");

            migrationBuilder.DropForeignKey(
                name: "FK_ProblemViews_Profiles_ViewerId",
                schema: "QA",
                table: "ProblemViews");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfileViews_Profiles_ViewedId",
                schema: "Profiles",
                table: "ProfileViews");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfileViews_Profiles_ViewerId",
                schema: "Profiles",
                table: "ProfileViews");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectViews_Profiles_ViewerId",
                schema: "Projects",
                table: "ProjectViews");

            migrationBuilder.AddForeignKey(
                name: "FK_PostViews_Profiles_ViewerId",
                schema: "ContentManagement",
                table: "PostViews",
                column: "ViewerId",
                principalSchema: "Profiles",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProblemViews_Profiles_ViewerId",
                schema: "QA",
                table: "ProblemViews",
                column: "ViewerId",
                principalSchema: "Profiles",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileViews_Profiles_ViewedId",
                schema: "Profiles",
                table: "ProfileViews",
                column: "ViewedId",
                principalSchema: "Profiles",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileViews_Profiles_ViewerId",
                schema: "Profiles",
                table: "ProfileViews",
                column: "ViewerId",
                principalSchema: "Profiles",
                principalTable: "Profiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectViews_Profiles_ViewerId",
                schema: "Projects",
                table: "ProjectViews",
                column: "ViewerId",
                principalSchema: "Profiles",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostViews_Profiles_ViewerId",
                schema: "ContentManagement",
                table: "PostViews");

            migrationBuilder.DropForeignKey(
                name: "FK_ProblemViews_Profiles_ViewerId",
                schema: "QA",
                table: "ProblemViews");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfileViews_Profiles_ViewedId",
                schema: "Profiles",
                table: "ProfileViews");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfileViews_Profiles_ViewerId",
                schema: "Profiles",
                table: "ProfileViews");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectViews_Profiles_ViewerId",
                schema: "Projects",
                table: "ProjectViews");

            migrationBuilder.AddForeignKey(
                name: "FK_PostViews_Profiles_ViewerId",
                schema: "ContentManagement",
                table: "PostViews",
                column: "ViewerId",
                principalSchema: "Profiles",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProblemViews_Profiles_ViewerId",
                schema: "QA",
                table: "ProblemViews",
                column: "ViewerId",
                principalSchema: "Profiles",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileViews_Profiles_ViewedId",
                schema: "Profiles",
                table: "ProfileViews",
                column: "ViewedId",
                principalSchema: "Profiles",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileViews_Profiles_ViewerId",
                schema: "Profiles",
                table: "ProfileViews",
                column: "ViewerId",
                principalSchema: "Profiles",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectViews_Profiles_ViewerId",
                schema: "Projects",
                table: "ProjectViews",
                column: "ViewerId",
                principalSchema: "Profiles",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
