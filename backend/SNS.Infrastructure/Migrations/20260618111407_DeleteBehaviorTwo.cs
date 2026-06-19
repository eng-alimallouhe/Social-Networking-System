using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SNS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DeleteBehaviorTwo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentReactions_Profiles_ReactorId",
                schema: "ContentManagement",
                table: "CommentReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                schema: "ContentManagement",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_ContentManagement_PostId",
                schema: "ContentManagement",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Profiles_AuthorId",
                schema: "ContentManagement",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityInvitations_Profiles_InviteeId",
                schema: "Communities",
                table: "CommunityInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityInvitations_Profiles_InviterId",
                schema: "Communities",
                table: "CommunityInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_ContentManagement_Communities_CommunityId",
                schema: "ContentManagement",
                table: "ContentManagement");

            migrationBuilder.DropForeignKey(
                name: "FK_ContentManagement_Profiles_AuthorId",
                schema: "ContentManagement",
                table: "ContentManagement");

            migrationBuilder.DropForeignKey(
                name: "FK_PostMedias_ContentManagement_PostId",
                schema: "ContentManagement",
                table: "PostMedias");

            migrationBuilder.DropForeignKey(
                name: "FK_PostReactions_ContentManagement_PostId",
                schema: "ContentManagement",
                table: "PostReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_PostReactions_Profiles_ReactorId",
                schema: "ContentManagement",
                table: "PostReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTags_ContentManagement_PostId",
                schema: "ContentManagement",
                table: "PostTags");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTopics_ContentManagement_PostId",
                schema: "ContentManagement",
                table: "PostTopics");

            migrationBuilder.DropForeignKey(
                name: "FK_PostViews_ContentManagement_PostId",
                schema: "ContentManagement",
                table: "PostViews");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedPosts_ContentManagement_PostId",
                schema: "ContentManagement",
                table: "SavedPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_SecuritySessions_Users_UserId",
                schema: "Identity",
                table: "SecuritySessions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserArchives_Users_PerformedById",
                schema: "Identity",
                table: "UserArchives");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContentManagement",
                schema: "ContentManagement",
                table: "ContentManagement");

            migrationBuilder.RenameTable(
                name: "ContentManagement",
                schema: "ContentManagement",
                newName: "Posts",
                newSchema: "ContentManagement");

            migrationBuilder.RenameIndex(
                name: "IX_ContentManagement_Title",
                schema: "ContentManagement",
                table: "Posts",
                newName: "IX_Posts_Title");

            migrationBuilder.RenameIndex(
                name: "IX_ContentManagement_CommunityId",
                schema: "ContentManagement",
                table: "Posts",
                newName: "IX_Posts_CommunityId");

            migrationBuilder.RenameIndex(
                name: "IX_ContentManagement_AuthorId",
                schema: "ContentManagement",
                table: "Posts",
                newName: "IX_Posts_AuthorId");

            migrationBuilder.AddColumn<bool>(
                name: "PurgeAllContentOnHardDelete",
                schema: "Identity",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Posts",
                schema: "ContentManagement",
                table: "Posts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReactions_Profiles_ReactorId",
                schema: "ContentManagement",
                table: "CommentReactions",
                column: "ReactorId",
                principalSchema: "Profiles",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                schema: "ContentManagement",
                table: "Comments",
                column: "ParentCommentId",
                principalSchema: "ContentManagement",
                principalTable: "Comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Posts_PostId",
                schema: "ContentManagement",
                table: "Comments",
                column: "PostId",
                principalSchema: "ContentManagement",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Profiles_AuthorId",
                schema: "ContentManagement",
                table: "Comments",
                column: "AuthorId",
                principalSchema: "Profiles",
                principalTable: "Profiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityInvitations_Profiles_InviteeId",
                schema: "Communities",
                table: "CommunityInvitations",
                column: "InviteeId",
                principalSchema: "Profiles",
                principalTable: "Profiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityInvitations_Profiles_InviterId",
                schema: "Communities",
                table: "CommunityInvitations",
                column: "InviterId",
                principalSchema: "Profiles",
                principalTable: "Profiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PostMedias_Posts_PostId",
                schema: "ContentManagement",
                table: "PostMedias",
                column: "PostId",
                principalSchema: "ContentManagement",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostReactions_Posts_PostId",
                schema: "ContentManagement",
                table: "PostReactions",
                column: "PostId",
                principalSchema: "ContentManagement",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostReactions_Profiles_ReactorId",
                schema: "ContentManagement",
                table: "PostReactions",
                column: "ReactorId",
                principalSchema: "Profiles",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Communities_CommunityId",
                schema: "ContentManagement",
                table: "Posts",
                column: "CommunityId",
                principalSchema: "Communities",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Profiles_AuthorId",
                schema: "ContentManagement",
                table: "Posts",
                column: "AuthorId",
                principalSchema: "Profiles",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PostTags_Posts_PostId",
                schema: "ContentManagement",
                table: "PostTags",
                column: "PostId",
                principalSchema: "ContentManagement",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostTopics_Posts_PostId",
                schema: "ContentManagement",
                table: "PostTopics",
                column: "PostId",
                principalSchema: "ContentManagement",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostViews_Posts_PostId",
                schema: "ContentManagement",
                table: "PostViews",
                column: "PostId",
                principalSchema: "ContentManagement",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedPosts_Posts_PostId",
                schema: "ContentManagement",
                table: "SavedPosts",
                column: "PostId",
                principalSchema: "ContentManagement",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SecuritySessions_Users_UserId",
                schema: "Identity",
                table: "SecuritySessions",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserArchives_Users_PerformedById",
                schema: "Identity",
                table: "UserArchives",
                column: "PerformedById",
                principalSchema: "Identity",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentReactions_Profiles_ReactorId",
                schema: "ContentManagement",
                table: "CommentReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                schema: "ContentManagement",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Posts_PostId",
                schema: "ContentManagement",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Profiles_AuthorId",
                schema: "ContentManagement",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityInvitations_Profiles_InviteeId",
                schema: "Communities",
                table: "CommunityInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunityInvitations_Profiles_InviterId",
                schema: "Communities",
                table: "CommunityInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_PostMedias_Posts_PostId",
                schema: "ContentManagement",
                table: "PostMedias");

            migrationBuilder.DropForeignKey(
                name: "FK_PostReactions_Posts_PostId",
                schema: "ContentManagement",
                table: "PostReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_PostReactions_Profiles_ReactorId",
                schema: "ContentManagement",
                table: "PostReactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Communities_CommunityId",
                schema: "ContentManagement",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Profiles_AuthorId",
                schema: "ContentManagement",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTags_Posts_PostId",
                schema: "ContentManagement",
                table: "PostTags");

            migrationBuilder.DropForeignKey(
                name: "FK_PostTopics_Posts_PostId",
                schema: "ContentManagement",
                table: "PostTopics");

            migrationBuilder.DropForeignKey(
                name: "FK_PostViews_Posts_PostId",
                schema: "ContentManagement",
                table: "PostViews");

            migrationBuilder.DropForeignKey(
                name: "FK_SavedPosts_Posts_PostId",
                schema: "ContentManagement",
                table: "SavedPosts");

            migrationBuilder.DropForeignKey(
                name: "FK_SecuritySessions_Users_UserId",
                schema: "Identity",
                table: "SecuritySessions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserArchives_Users_PerformedById",
                schema: "Identity",
                table: "UserArchives");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Posts",
                schema: "ContentManagement",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "PurgeAllContentOnHardDelete",
                schema: "Identity",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Posts",
                schema: "ContentManagement",
                newName: "ContentManagement",
                newSchema: "ContentManagement");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_Title",
                schema: "ContentManagement",
                table: "ContentManagement",
                newName: "IX_ContentManagement_Title");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_CommunityId",
                schema: "ContentManagement",
                table: "ContentManagement",
                newName: "IX_ContentManagement_CommunityId");

            migrationBuilder.RenameIndex(
                name: "IX_Posts_AuthorId",
                schema: "ContentManagement",
                table: "ContentManagement",
                newName: "IX_ContentManagement_AuthorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContentManagement",
                schema: "ContentManagement",
                table: "ContentManagement",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReactions_Profiles_ReactorId",
                schema: "ContentManagement",
                table: "CommentReactions",
                column: "ReactorId",
                principalSchema: "Profiles",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Comments_ParentCommentId",
                schema: "ContentManagement",
                table: "Comments",
                column: "ParentCommentId",
                principalSchema: "ContentManagement",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_ContentManagement_PostId",
                schema: "ContentManagement",
                table: "Comments",
                column: "PostId",
                principalSchema: "ContentManagement",
                principalTable: "ContentManagement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Profiles_AuthorId",
                schema: "ContentManagement",
                table: "Comments",
                column: "AuthorId",
                principalSchema: "Profiles",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityInvitations_Profiles_InviteeId",
                schema: "Communities",
                table: "CommunityInvitations",
                column: "InviteeId",
                principalSchema: "Profiles",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CommunityInvitations_Profiles_InviterId",
                schema: "Communities",
                table: "CommunityInvitations",
                column: "InviterId",
                principalSchema: "Profiles",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContentManagement_Communities_CommunityId",
                schema: "ContentManagement",
                table: "ContentManagement",
                column: "CommunityId",
                principalSchema: "Communities",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ContentManagement_Profiles_AuthorId",
                schema: "ContentManagement",
                table: "ContentManagement",
                column: "AuthorId",
                principalSchema: "Profiles",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PostMedias_ContentManagement_PostId",
                schema: "ContentManagement",
                table: "PostMedias",
                column: "PostId",
                principalSchema: "ContentManagement",
                principalTable: "ContentManagement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostReactions_ContentManagement_PostId",
                schema: "ContentManagement",
                table: "PostReactions",
                column: "PostId",
                principalSchema: "ContentManagement",
                principalTable: "ContentManagement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostReactions_Profiles_ReactorId",
                schema: "ContentManagement",
                table: "PostReactions",
                column: "ReactorId",
                principalSchema: "Profiles",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PostTags_ContentManagement_PostId",
                schema: "ContentManagement",
                table: "PostTags",
                column: "PostId",
                principalSchema: "ContentManagement",
                principalTable: "ContentManagement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostTopics_ContentManagement_PostId",
                schema: "ContentManagement",
                table: "PostTopics",
                column: "PostId",
                principalSchema: "ContentManagement",
                principalTable: "ContentManagement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PostViews_ContentManagement_PostId",
                schema: "ContentManagement",
                table: "PostViews",
                column: "PostId",
                principalSchema: "ContentManagement",
                principalTable: "ContentManagement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SavedPosts_ContentManagement_PostId",
                schema: "ContentManagement",
                table: "SavedPosts",
                column: "PostId",
                principalSchema: "ContentManagement",
                principalTable: "ContentManagement",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SecuritySessions_Users_UserId",
                schema: "Identity",
                table: "SecuritySessions",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserArchives_Users_PerformedById",
                schema: "Identity",
                table: "UserArchives",
                column: "PerformedById",
                principalSchema: "Identity",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
