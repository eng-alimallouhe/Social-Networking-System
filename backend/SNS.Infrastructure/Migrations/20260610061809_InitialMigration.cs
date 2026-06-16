using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SNS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Education");

            migrationBuilder.EnsureSchema(
                name: "Profiles");

            migrationBuilder.EnsureSchema(
                name: "ContentManagement");

            migrationBuilder.EnsureSchema(
                name: "Communities");

            migrationBuilder.EnsureSchema(
                name: "Jobs");

            migrationBuilder.EnsureSchema(
                name: "Identity");

            migrationBuilder.EnsureSchema(
                name: "QA");

            migrationBuilder.EnsureSchema(
                name: "EventsHolder");

            migrationBuilder.EnsureSchema(
                name: "ProfileContext");

            migrationBuilder.EnsureSchema(
                name: "Projects");

            migrationBuilder.EnsureSchema(
                name: "Resumes");

            migrationBuilder.EnsureSchema(
                name: "Preferences");

            migrationBuilder.CreateTable(
                name: "Companies",
                schema: "Jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Industry = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    WebsiteUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "EventsHolder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OccurredOnUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProcessedOnUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkillsCategories",
                schema: "Preferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillsCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                schema: "Preferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Topics",
                schema: "Preferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Universities",
                schema: "Education",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Universities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                schema: "Jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    MinSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nchar(3)", fixedLength: true, maxLength: 3, nullable: false),
                    SalaryType = table.Column<int>(type: "int", nullable: false),
                    KeyResponsibilitiesText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jobs_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "Jobs",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    FailedLoginAttempts = table.Column<int>(type: "int", nullable: false),
                    PreferredLanguage = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLogIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastPasswordChange = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsBanned = table.Column<bool>(type: "bit", nullable: false),
                    IsSuspended = table.Column<bool>(type: "bit", nullable: false),
                    SuspendedUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SuspensionReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeactivated = table.Column<bool>(type: "bit", nullable: false),
                    DeactivatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    CodeCreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                schema: "Preferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skills_SkillsCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "Preferences",
                        principalTable: "SkillsCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TopicInterest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TopicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicInterest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopicInterest_Topics_TopicId",
                        column: x => x.TopicId,
                        principalSchema: "Preferences",
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityArchives",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OldUserIdentifier = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NewUserIdentifier = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityArchives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityArchives_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActorProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Source = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    TargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PasswordArchives",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordArchives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordArchives_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                schema: "Profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProfilePictureUrl = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true),
                    Specialization = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GitHubUrl = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true),
                    LinkedInUrl = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true),
                    FacebookUrl = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true),
                    XUrl = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true),
                    Website = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SkillsSummary = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Reputation = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profiles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserArchives",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PerformedById = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Parameters = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserArchives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserArchives_Users_PerformedById",
                        column: x => x.PerformedById,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserArchives_Users_TargetId",
                        column: x => x.TargetId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserNotificationPreferences",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NewFollower = table.Column<bool>(type: "bit", nullable: false),
                    PostLikes = table.Column<bool>(type: "bit", nullable: false),
                    PostComments = table.Column<bool>(type: "bit", nullable: false),
                    CommentReplies = table.Column<bool>(type: "bit", nullable: false),
                    Mentions = table.Column<bool>(type: "bit", nullable: false),
                    Messages = table.Column<bool>(type: "bit", nullable: false),
                    CommunityPosts = table.Column<bool>(type: "bit", nullable: false),
                    CommunityAnnouncements = table.Column<bool>(type: "bit", nullable: false),
                    ProjectInvitations = table.Column<bool>(type: "bit", nullable: false),
                    ProjectUpdates = table.Column<bool>(type: "bit", nullable: false),
                    ProblemSolutions = table.Column<bool>(type: "bit", nullable: false),
                    LoginAlerts = table.Column<bool>(type: "bit", nullable: false),
                    PasswordChanged = table.Column<bool>(type: "bit", nullable: false),
                    EnableEmailNotifications = table.Column<bool>(type: "bit", nullable: false),
                    EnableSmsNotifications = table.Column<bool>(type: "bit", nullable: false),
                    EnablePushNotifications = table.Column<bool>(type: "bit", nullable: false),
                    EnableInAppNotifications = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserNotificationPreferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserNotificationPreferences_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPassKeys",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CredentialId = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PublicKey = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    DeviceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignatureCounter = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPassKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPassKeys_Users_Id",
                        column: x => x.Id,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersSecuritySettings",
                schema: "Identity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecoveryEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FailedLoginNotifications = table.Column<bool>(type: "bit", nullable: false),
                    LoginAlerts = table.Column<bool>(type: "bit", nullable: false),
                    PasswordChangeAlerts = table.Column<bool>(type: "bit", nullable: false),
                    AuthenticatorSecretKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MfaProvider = table.Column<int>(type: "int", nullable: false),
                    DefaultCommunicationMethod = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersSecuritySettings", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UsersSecuritySettings_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobSkills",
                schema: "Jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSkills_Jobs_JobId",
                        column: x => x.JobId,
                        principalSchema: "Jobs",
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "Preferences",
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AcademicRecords",
                schema: "Education",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UniversityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Degree = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FieldOfStudy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademicRecords_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AcademicRecords_Universities_UniversityId",
                        column: x => x.UniversityId,
                        principalSchema: "Education",
                        principalTable: "Universities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Blocks",
                schema: "Profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlockerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlockedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blocks_Profiles_BlockedId",
                        column: x => x.BlockedId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Blocks_Profiles_BlockerId",
                        column: x => x.BlockerId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Communities",
                schema: "Communities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RulesText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Policy = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    LogoUrl = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Communities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Communities_Profiles_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyAdministrators",
                schema: "Jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdminRole = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyAdministrators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyAdministrators_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalSchema: "Jobs",
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyAdministrators_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Follows",
                schema: "Profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FollowerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FollowingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Follows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Follows_Profiles_FollowerId",
                        column: x => x.FollowerId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Follows_Profiles_FollowingId",
                        column: x => x.FollowingId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "JobApplications",
                schema: "Jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResumeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CoverLetterText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResumeFileUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobApplications_Jobs_JobId",
                        column: x => x.JobId,
                        principalSchema: "Jobs",
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobApplications_Profiles_ApplicantId",
                        column: x => x.ApplicantId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Mutes",
                schema: "Profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MutedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MuterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mutes_Profiles_MutedId",
                        column: x => x.MutedId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Mutes_Profiles_MuterId",
                        column: x => x.MuterId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProfileSkills",
                schema: "ProfileContext",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileSkills_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "Preferences",
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileTopics",
                schema: "ProfileContext",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TopicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<double>(type: "float", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileTopics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileTopics_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileTopics_Topics_TopicId",
                        column: x => x.TopicId,
                        principalSchema: "Preferences",
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileViews",
                schema: "Profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ViewedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ViewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileViews_Profiles_ViewedId",
                        column: x => x.ViewedId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileViews_Profiles_ViewerId",
                        column: x => x.ViewerId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                schema: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MainImageUrl = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false),
                    ReadmeContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GitHubUrl = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false),
                    LiveDemoUrl = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SourceCodeTree = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Profiles_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReputationLedgers",
                schema: "Profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActionType = table.Column<int>(type: "int", nullable: false),
                    PointsDelta = table.Column<int>(type: "int", nullable: false),
                    SourceEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReputationLedgers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReputationLedgers_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resumes",
                schema: "Resumes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonalPictureUrl = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true),
                    SyncProfilePicture = table.Column<bool>(type: "bit", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Template = table.Column<int>(type: "int", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Langauge = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resumes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resumes_Profiles_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedJobs",
                schema: "Jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedJobs_Jobs_JobId",
                        column: x => x.JobId,
                        principalSchema: "Jobs",
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedJobs_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedProfiles",
                schema: "Profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SaverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SavedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedProfiles_Profiles_SavedId",
                        column: x => x.SavedId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SavedProfiles_Profiles_SaverId",
                        column: x => x.SaverId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceToken = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    FriendlyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Browser = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OperatingSystem = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DeviceVendor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeviceModel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FingerprintHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    IsTrusted = table.Column<bool>(type: "bit", nullable: false),
                    FirstSeenAt = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    LastSeenAt = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    UserSecuritySettingsUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devices_UsersSecuritySettings_UserSecuritySettingsUserId",
                        column: x => x.UserSecuritySettingsUserId,
                        principalSchema: "Identity",
                        principalTable: "UsersSecuritySettings",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Devices_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecoveryCodes",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserSecuritySettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CodeHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecoveryCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecoveryCodes_UsersSecuritySettings_UserSecuritySettingsId",
                        column: x => x.UserSecuritySettingsId,
                        principalSchema: "Identity",
                        principalTable: "UsersSecuritySettings",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommunityAuditLogs",
                schema: "Communities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PerformedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityAuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityAuditLogs_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalSchema: "Communities",
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommunityAuditLogs_Profiles_ActorId",
                        column: x => x.ActorId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CommunityInvitations",
                schema: "Communities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InviterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InviteeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityInvitations_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalSchema: "Communities",
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommunityInvitations_Profiles_InviteeId",
                        column: x => x.InviteeId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommunityInvitations_Profiles_InviterId",
                        column: x => x.InviterId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommunityJoinRequests",
                schema: "Communities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmitterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityJoinRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityJoinRequests_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalSchema: "Communities",
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommunityJoinRequests_Profiles_SubmitterId",
                        column: x => x.SubmitterId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommunityMemberships",
                schema: "Communities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    JoinedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityMemberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityMemberships_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalSchema: "Communities",
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommunityMemberships_Profiles_MemberId",
                        column: x => x.MemberId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommunityRules",
                schema: "Communities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityRules_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalSchema: "Communities",
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommunitySettings",
                schema: "Communities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllowPostWithoutApproval = table.Column<bool>(type: "bit", nullable: false),
                    AllowInvitationsByMembers = table.Column<bool>(type: "bit", nullable: false),
                    AllowComments = table.Column<bool>(type: "bit", nullable: false),
                    AllowMediaUpload = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunitySettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunitySettings_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalSchema: "Communities",
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentManagement",
                schema: "ContentManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPinned = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: true),
                    EngagementScore = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastInteractedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentManagement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentManagement_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalSchema: "Communities",
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ContentManagement_Profiles_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Problems",
                schema: "QA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Problems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Problems_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalSchema: "Communities",
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Problems_Profiles_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectContributors",
                schema: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContributorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvitingStatus = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    InvitationSentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InvitationMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectContributors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectContributors_Profiles_ContributorId",
                        column: x => x.ContributorId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectContributors_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Projects",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMedias",
                schema: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MediaUrl = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectMedias_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Projects",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMilestones",
                schema: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMilestones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectMilestones_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Projects",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectRatings",
                schema: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RaterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RatingValue = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectRatings_Profiles_RaterId",
                        column: x => x.RaterId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectRatings_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Projects",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectSkills",
                schema: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectSkills_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Projects",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalSchema: "Preferences",
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTags",
                schema: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectTags_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Projects",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectTags_Tags_TagId",
                        column: x => x.TagId,
                        principalSchema: "Preferences",
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectViews",
                schema: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ViewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeviceType = table.Column<int>(type: "int", nullable: true),
                    IpHash = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectViews_Profiles_ViewerId",
                        column: x => x.ViewerId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectViews_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Projects",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedProjects",
                schema: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedProjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedProjects_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedProjects_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Projects",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResumeCertificates",
                schema: "Resumes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResumeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Issuer = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeCertificates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResumeCertificates_Resumes_ResumeId",
                        column: x => x.ResumeId,
                        principalSchema: "Resumes",
                        principalTable: "Resumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResumeEducations",
                schema: "Resumes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResumeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UniversityName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FacultyName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Degree = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FieldOfStudy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GPA = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeEducations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResumeEducations_Resumes_ResumeId",
                        column: x => x.ResumeId,
                        principalSchema: "Resumes",
                        principalTable: "Resumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResumeExperiences",
                schema: "Resumes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResumeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Position = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeExperiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResumeExperiences_Resumes_ResumeId",
                        column: x => x.ResumeId,
                        principalSchema: "Resumes",
                        principalTable: "Resumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResumeLanguages",
                schema: "Resumes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResumeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Language = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeLanguages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResumeLanguages_Resumes_ResumeId",
                        column: x => x.ResumeId,
                        principalSchema: "Resumes",
                        principalTable: "Resumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResumeProjects",
                schema: "Resumes",
                columns: table => new
                {
                    ResumeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeProjects", x => new { x.ResumeId, x.ProjectId });
                    table.ForeignKey(
                        name: "FK_ResumeProjects_Resumes_ResumeId",
                        column: x => x.ResumeId,
                        principalSchema: "Resumes",
                        principalTable: "Resumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResumeSkills",
                schema: "Resumes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResumeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResumeSkills_Resumes_ResumeId",
                        column: x => x.ResumeId,
                        principalSchema: "Resumes",
                        principalTable: "Resumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SecuritySessions",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeviceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastSeenAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LogoutAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IpAddress = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    RevokedReason = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecuritySessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecuritySessions_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalSchema: "Identity",
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SecuritySessions_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                schema: "ContentManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentCommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalSchema: "ContentManagement",
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_ContentManagement_PostId",
                        column: x => x.PostId,
                        principalSchema: "ContentManagement",
                        principalTable: "ContentManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Profiles_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostMedias",
                schema: "ContentManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false),
                    MimeType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true),
                    Duration = table.Column<double>(type: "float", nullable: true),
                    Width = table.Column<int>(type: "int", nullable: true),
                    Height = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostMedias_ContentManagement_PostId",
                        column: x => x.PostId,
                        principalSchema: "ContentManagement",
                        principalTable: "ContentManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostReactions",
                schema: "ContentManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReactorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostReactions_ContentManagement_PostId",
                        column: x => x.PostId,
                        principalSchema: "ContentManagement",
                        principalTable: "ContentManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostReactions_Profiles_ReactorId",
                        column: x => x.ReactorId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostTags",
                schema: "ContentManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Confidence = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostTags_ContentManagement_PostId",
                        column: x => x.PostId,
                        principalSchema: "ContentManagement",
                        principalTable: "ContentManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostTags_Tags_TagId",
                        column: x => x.TagId,
                        principalSchema: "Preferences",
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostTopics",
                schema: "ContentManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TopicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Confidence = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostTopics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostTopics_ContentManagement_PostId",
                        column: x => x.PostId,
                        principalSchema: "ContentManagement",
                        principalTable: "ContentManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostTopics_Topics_TopicId",
                        column: x => x.TopicId,
                        principalSchema: "Preferences",
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostViews",
                schema: "ContentManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ViewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeviceType = table.Column<int>(type: "int", nullable: true),
                    IpHash = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostViews_ContentManagement_PostId",
                        column: x => x.PostId,
                        principalSchema: "ContentManagement",
                        principalTable: "ContentManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostViews_Profiles_ViewerId",
                        column: x => x.ViewerId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SavedPosts",
                schema: "ContentManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedPosts_ContentManagement_PostId",
                        column: x => x.PostId,
                        principalSchema: "ContentManagement",
                        principalTable: "ContentManagement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedPosts_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProblemContentBlocks",
                schema: "QA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProblemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExtraInfo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemContentBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProblemContentBlocks_Problems_ProblemId",
                        column: x => x.ProblemId,
                        principalSchema: "QA",
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProblemTags",
                schema: "QA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProblemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProblemTags_Problems_ProblemId",
                        column: x => x.ProblemId,
                        principalSchema: "QA",
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProblemTags_Tags_TagId",
                        column: x => x.TagId,
                        principalSchema: "Preferences",
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProblemTopics",
                schema: "QA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProblemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TopicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Confidence = table.Column<float>(type: "real", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemTopics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProblemTopics_Problems_ProblemId",
                        column: x => x.ProblemId,
                        principalSchema: "QA",
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProblemTopics_Topics_TopicId",
                        column: x => x.TopicId,
                        principalSchema: "Preferences",
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProblemViews",
                schema: "QA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProblemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ViewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeviceType = table.Column<int>(type: "int", nullable: true),
                    IpHash = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProblemViews_Problems_ProblemId",
                        column: x => x.ProblemId,
                        principalSchema: "QA",
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProblemViews_Profiles_ViewerId",
                        column: x => x.ViewerId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProblemVotes",
                schema: "QA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProblemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProblemVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProblemVotes_Problems_ProblemId",
                        column: x => x.ProblemId,
                        principalSchema: "QA",
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProblemVotes_Profiles_VoterId",
                        column: x => x.VoterId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedProblems",
                schema: "QA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProblemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedProblems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedProblems_Problems_ProblemId",
                        column: x => x.ProblemId,
                        principalSchema: "QA",
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedProblems_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Solutions",
                schema: "QA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProblemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Solutions_Problems_ProblemId",
                        column: x => x.ProblemId,
                        principalSchema: "QA",
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Solutions_Profiles_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SecuritySessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_SecuritySessions_SecuritySessionId",
                        column: x => x.SecuritySessionId,
                        principalSchema: "Identity",
                        principalTable: "SecuritySessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommentMedias",
                schema: "ContentManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false),
                    MimeType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration = table.Column<double>(type: "float", nullable: true),
                    Width = table.Column<int>(type: "int", nullable: true),
                    Height = table.Column<int>(type: "int", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "CommentReactions",
                schema: "ContentManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReactorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentReactions_Comments_CommentId",
                        column: x => x.CommentId,
                        principalSchema: "ContentManagement",
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentReactions_Profiles_ReactorId",
                        column: x => x.ReactorId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DiscussionsDI",
                schema: "QA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SolutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentDiscussionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CodeLanguage = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscussionsDI", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscussionsDI_DiscussionsDI_ParentDiscussionId",
                        column: x => x.ParentDiscussionId,
                        principalSchema: "QA",
                        principalTable: "DiscussionsDI",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiscussionsDI_Profiles_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DiscussionsDI_Solutions_SolutionId",
                        column: x => x.SolutionId,
                        principalSchema: "QA",
                        principalTable: "Solutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SavedSolutions",
                schema: "QA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SolutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SavedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SavedSolutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SavedSolutions_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SavedSolutions_Solutions_SolutionId",
                        column: x => x.SolutionId,
                        principalSchema: "QA",
                        principalTable: "Solutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolutionContentBlocks",
                schema: "QA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SolutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExtraInfo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolutionContentBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolutionContentBlocks_Solutions_SolutionId",
                        column: x => x.SolutionId,
                        principalSchema: "QA",
                        principalTable: "Solutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolutionVotes",
                schema: "QA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SolutionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VoterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolutionVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolutionVotes_Profiles_VoterId",
                        column: x => x.VoterId,
                        principalSchema: "Profiles",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolutionVotes_Solutions_SolutionId",
                        column: x => x.SolutionId,
                        principalSchema: "QA",
                        principalTable: "Solutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicRecords_ProfileId",
                schema: "Education",
                table: "AcademicRecords",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicRecords_UniversityId",
                schema: "Education",
                table: "AcademicRecords",
                column: "UniversityId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicRecords_UniversityId_ProfileId",
                schema: "Education",
                table: "AcademicRecords",
                columns: new[] { "UniversityId", "ProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_BlockedId",
                schema: "Profiles",
                table: "Blocks",
                column: "BlockedId");

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_BlockerId",
                schema: "Profiles",
                table: "Blocks",
                column: "BlockerId");

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_BlockerId_BlockedId",
                schema: "Profiles",
                table: "Blocks",
                columns: new[] { "BlockerId", "BlockedId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentMedias_CommentId",
                schema: "ContentManagement",
                table: "CommentMedias",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReactions_CommentId",
                schema: "ContentManagement",
                table: "CommentReactions",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReactions_CommentId_ReactorId",
                schema: "ContentManagement",
                table: "CommentReactions",
                columns: new[] { "CommentId", "ReactorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentReactions_ReactorId",
                schema: "ContentManagement",
                table: "CommentReactions",
                column: "ReactorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorId",
                schema: "ContentManagement",
                table: "Comments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentCommentId",
                schema: "ContentManagement",
                table: "Comments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                schema: "ContentManagement",
                table: "Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_Name",
                schema: "Communities",
                table: "Communities",
                column: "Name",
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_OwnerId",
                schema: "Communities",
                table: "Communities",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityAuditLogs_ActorId",
                schema: "Communities",
                table: "CommunityAuditLogs",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityAuditLogs_CommunityId",
                schema: "Communities",
                table: "CommunityAuditLogs",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityInvitations_CommunityId",
                schema: "Communities",
                table: "CommunityInvitations",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityInvitations_CommunityId_InviteeId",
                schema: "Communities",
                table: "CommunityInvitations",
                columns: new[] { "CommunityId", "InviteeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommunityInvitations_InviteeId",
                schema: "Communities",
                table: "CommunityInvitations",
                column: "InviteeId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityInvitations_InviterId",
                schema: "Communities",
                table: "CommunityInvitations",
                column: "InviterId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityJoinRequests_CommunityId_SubmitterId",
                schema: "Communities",
                table: "CommunityJoinRequests",
                columns: new[] { "CommunityId", "SubmitterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommunityJoinRequests_SubmitterId",
                schema: "Communities",
                table: "CommunityJoinRequests",
                column: "SubmitterId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityMemberships_CommunityId",
                schema: "Communities",
                table: "CommunityMemberships",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityMemberships_CommunityId_MemberId",
                schema: "Communities",
                table: "CommunityMemberships",
                columns: new[] { "CommunityId", "MemberId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommunityMemberships_MemberId",
                schema: "Communities",
                table: "CommunityMemberships",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityRules_CommunityId",
                schema: "Communities",
                table: "CommunityRules",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunitySettings_CommunityId",
                schema: "Communities",
                table: "CommunitySettings",
                column: "CommunityId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAdministrators_CompanyId",
                schema: "Jobs",
                table: "CompanyAdministrators",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAdministrators_ProfileId_CompanyId",
                schema: "Jobs",
                table: "CompanyAdministrators",
                columns: new[] { "ProfileId", "CompanyId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentManagement_AuthorId",
                schema: "ContentManagement",
                table: "ContentManagement",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentManagement_CommunityId",
                schema: "ContentManagement",
                table: "ContentManagement",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentManagement_Title",
                schema: "ContentManagement",
                table: "ContentManagement",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_DeviceToken",
                schema: "Identity",
                table: "Devices",
                column: "DeviceToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_UserId",
                schema: "Identity",
                table: "Devices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_UserSecuritySettingsUserId",
                schema: "Identity",
                table: "Devices",
                column: "UserSecuritySettingsUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionsDI_AuthorId",
                schema: "QA",
                table: "DiscussionsDI",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionsDI_ParentDiscussionId",
                schema: "QA",
                table: "DiscussionsDI",
                column: "ParentDiscussionId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscussionsDI_SolutionId",
                schema: "QA",
                table: "DiscussionsDI",
                column: "SolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Follows_FollowerId",
                schema: "Profiles",
                table: "Follows",
                column: "FollowerId");

            migrationBuilder.CreateIndex(
                name: "IX_Follows_FollowerId_FollowingId",
                schema: "Profiles",
                table: "Follows",
                columns: new[] { "FollowerId", "FollowingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Follows_FollowingId",
                schema: "Profiles",
                table: "Follows",
                column: "FollowingId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityArchives_UserId",
                schema: "Identity",
                table: "IdentityArchives",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_ApplicantId",
                schema: "Jobs",
                table: "JobApplications",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_JobId_ApplicantId",
                schema: "Jobs",
                table: "JobApplications",
                columns: new[] { "JobId", "ApplicantId" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_CompanyId",
                schema: "Jobs",
                table: "Jobs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_Title",
                schema: "Jobs",
                table: "Jobs",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_JobSkills_JobId_SkillId",
                schema: "Jobs",
                table: "JobSkills",
                columns: new[] { "JobId", "SkillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobSkills_SkillId",
                schema: "Jobs",
                table: "JobSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_Mutes_MutedId",
                schema: "Profiles",
                table: "Mutes",
                column: "MutedId");

            migrationBuilder.CreateIndex(
                name: "IX_Mutes_MuterId_MutedId",
                schema: "Profiles",
                table: "Mutes",
                columns: new[] { "MuterId", "MutedId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TargetId",
                schema: "Identity",
                table: "Notifications",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                schema: "Identity",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessages_Unprocessed",
                schema: "EventsHolder",
                table: "OutboxMessages",
                columns: new[] { "ProcessedOnUtc", "OccurredOnUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordArchives_UserId",
                schema: "Identity",
                table: "PasswordArchives",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PostMedias_PostId",
                schema: "ContentManagement",
                table: "PostMedias",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReactions_PostId",
                schema: "ContentManagement",
                table: "PostReactions",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReactions_PostId_ReactorId",
                schema: "ContentManagement",
                table: "PostReactions",
                columns: new[] { "PostId", "ReactorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostReactions_ReactorId",
                schema: "ContentManagement",
                table: "PostReactions",
                column: "ReactorId");

            migrationBuilder.CreateIndex(
                name: "IX_PostTags_PostId_TagId",
                schema: "ContentManagement",
                table: "PostTags",
                columns: new[] { "PostId", "TagId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostTags_TagId",
                schema: "ContentManagement",
                table: "PostTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_PostTopics_PostId_TopicId",
                schema: "ContentManagement",
                table: "PostTopics",
                columns: new[] { "PostId", "TopicId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostTopics_TopicId",
                schema: "ContentManagement",
                table: "PostTopics",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_PostViews_PostId",
                schema: "ContentManagement",
                table: "PostViews",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostViews_PostId_ViewerId",
                schema: "ContentManagement",
                table: "PostViews",
                columns: new[] { "PostId", "ViewerId" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_PostViews_ViewerId",
                schema: "ContentManagement",
                table: "PostViews",
                column: "ViewerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemContentBlocks_ProblemId",
                schema: "QA",
                table: "ProblemContentBlocks",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_AuthorId",
                schema: "QA",
                table: "Problems",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_CommunityId",
                schema: "QA",
                table: "Problems",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_Title",
                schema: "QA",
                table: "Problems",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemTags_ProblemId",
                schema: "QA",
                table: "ProblemTags",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemTags_ProblemId_TagId",
                schema: "QA",
                table: "ProblemTags",
                columns: new[] { "ProblemId", "TagId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProblemTags_TagId",
                schema: "QA",
                table: "ProblemTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemTopics_ProblemId_TopicId",
                schema: "QA",
                table: "ProblemTopics",
                columns: new[] { "ProblemId", "TopicId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProblemTopics_TopicId",
                schema: "QA",
                table: "ProblemTopics",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemViews_ProblemId",
                schema: "QA",
                table: "ProblemViews",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemViews_ProblemId_ViewerId",
                schema: "QA",
                table: "ProblemViews",
                columns: new[] { "ProblemId", "ViewerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProblemViews_ViewerId",
                schema: "QA",
                table: "ProblemViews",
                column: "ViewerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemVotes_ProblemId",
                schema: "QA",
                table: "ProblemVotes",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemVotes_ProblemId_VoterId",
                schema: "QA",
                table: "ProblemVotes",
                columns: new[] { "ProblemId", "VoterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProblemVotes_VoterId",
                schema: "QA",
                table: "ProblemVotes",
                column: "VoterId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_FullName",
                schema: "Profiles",
                table: "Profiles",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_UserId",
                schema: "Profiles",
                table: "Profiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSkills_ProfileId",
                schema: "ProfileContext",
                table: "ProfileSkills",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSkills_ProfileId_SkillId",
                schema: "ProfileContext",
                table: "ProfileSkills",
                columns: new[] { "ProfileId", "SkillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSkills_SkillId",
                schema: "ProfileContext",
                table: "ProfileSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileTopics_ProfileId",
                schema: "ProfileContext",
                table: "ProfileTopics",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileTopics_ProfileId_TopicId",
                schema: "ProfileContext",
                table: "ProfileTopics",
                columns: new[] { "ProfileId", "TopicId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileTopics_TopicId",
                schema: "ProfileContext",
                table: "ProfileTopics",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileViews_ViewedId",
                schema: "Profiles",
                table: "ProfileViews",
                column: "ViewedId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileViews_ViewedId_ViewerId",
                schema: "Profiles",
                table: "ProfileViews",
                columns: new[] { "ViewedId", "ViewerId" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileViews_ViewerId",
                schema: "Profiles",
                table: "ProfileViews",
                column: "ViewerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectContributors_ContributorId",
                schema: "Projects",
                table: "ProjectContributors",
                column: "ContributorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectContributors_ProjectId",
                schema: "Projects",
                table: "ProjectContributors",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectContributors_ProjectId_ContributorId",
                schema: "Projects",
                table: "ProjectContributors",
                columns: new[] { "ProjectId", "ContributorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMedias_ProjectId",
                schema: "Projects",
                table: "ProjectMedias",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMilestones_ProjectId",
                schema: "Projects",
                table: "ProjectMilestones",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRatings_ProjectId",
                schema: "Projects",
                table: "ProjectRatings",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRatings_ProjectId_RaterId",
                schema: "Projects",
                table: "ProjectRatings",
                columns: new[] { "ProjectId", "RaterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRatings_RaterId",
                schema: "Projects",
                table: "ProjectRatings",
                column: "RaterId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OwnerId",
                schema: "Projects",
                table: "Projects",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Title",
                schema: "Projects",
                table: "Projects",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSkills_ProjectId",
                schema: "Projects",
                table: "ProjectSkills",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSkills_ProjectId_SkillId",
                schema: "Projects",
                table: "ProjectSkills",
                columns: new[] { "ProjectId", "SkillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSkills_SkillId",
                schema: "Projects",
                table: "ProjectSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTags_ProjectId",
                schema: "Projects",
                table: "ProjectTags",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTags_ProjectId_TagId",
                schema: "Projects",
                table: "ProjectTags",
                columns: new[] { "ProjectId", "TagId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTags_TagId",
                schema: "Projects",
                table: "ProjectTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectViews_ProjectId",
                schema: "Projects",
                table: "ProjectViews",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectViews_ProjectId_ViewerId",
                schema: "Projects",
                table: "ProjectViews",
                columns: new[] { "ProjectId", "ViewerId" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectViews_ViewerId",
                schema: "Projects",
                table: "ProjectViews",
                column: "ViewerId");

            migrationBuilder.CreateIndex(
                name: "IX_RecoveryCodes_CodeHash",
                schema: "Identity",
                table: "RecoveryCodes",
                column: "CodeHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecoveryCodes_UserSecuritySettingsId",
                schema: "Identity",
                table: "RecoveryCodes",
                column: "UserSecuritySettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_SecuritySessionId",
                schema: "Identity",
                table: "RefreshTokens",
                column: "SecuritySessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReputationLedgers_ProfileId",
                schema: "Profiles",
                table: "ReputationLedgers",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeCertificates_ResumeId",
                schema: "Resumes",
                table: "ResumeCertificates",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeEducations_ResumeId",
                schema: "Resumes",
                table: "ResumeEducations",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeExperiences_ResumeId",
                schema: "Resumes",
                table: "ResumeExperiences",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeLanguages_ResumeId",
                schema: "Resumes",
                table: "ResumeLanguages",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeLanguages_ResumeId_Language",
                schema: "Resumes",
                table: "ResumeLanguages",
                columns: new[] { "ResumeId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_OwnerId",
                schema: "Resumes",
                table: "Resumes",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_Title",
                schema: "Resumes",
                table: "Resumes",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeSkills_ResumeId",
                schema: "Resumes",
                table: "ResumeSkills",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeSkills_ResumeId_SkillName",
                schema: "Resumes",
                table: "ResumeSkills",
                columns: new[] { "ResumeId", "SkillName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavedJobs_JobId",
                schema: "Jobs",
                table: "SavedJobs",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedJobs_ProfileId_JobId",
                schema: "Jobs",
                table: "SavedJobs",
                columns: new[] { "ProfileId", "JobId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavedJobs_ProfileId_SavedAt",
                schema: "Jobs",
                table: "SavedJobs",
                columns: new[] { "ProfileId", "SavedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SavedPosts_PostId",
                schema: "ContentManagement",
                table: "SavedPosts",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedPosts_ProfileId_PostId",
                schema: "ContentManagement",
                table: "SavedPosts",
                columns: new[] { "ProfileId", "PostId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavedProblems_ProblemId",
                schema: "QA",
                table: "SavedProblems",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedProblems_ProfileId_ProblemId",
                schema: "QA",
                table: "SavedProblems",
                columns: new[] { "ProfileId", "ProblemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavedProblems_ProfileId_SavedAt",
                schema: "QA",
                table: "SavedProblems",
                columns: new[] { "ProfileId", "SavedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SavedProfiles_SavedId",
                schema: "Profiles",
                table: "SavedProfiles",
                column: "SavedId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedProfiles_SaverId_SavedId",
                schema: "Profiles",
                table: "SavedProfiles",
                columns: new[] { "SaverId", "SavedId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavedProjects_ProfileId_ProjectId",
                schema: "Projects",
                table: "SavedProjects",
                columns: new[] { "ProfileId", "ProjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavedProjects_ProfileId_SavedAt",
                schema: "Projects",
                table: "SavedProjects",
                columns: new[] { "ProfileId", "SavedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SavedProjects_ProjectId",
                schema: "Projects",
                table: "SavedProjects",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SavedSolutions_ProfileId_SavedAt",
                schema: "QA",
                table: "SavedSolutions",
                columns: new[] { "ProfileId", "SavedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SavedSolutions_ProfileId_SolutionId",
                schema: "QA",
                table: "SavedSolutions",
                columns: new[] { "ProfileId", "SolutionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SavedSolutions_SolutionId",
                schema: "QA",
                table: "SavedSolutions",
                column: "SolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_SecuritySessions_DeviceId",
                schema: "Identity",
                table: "SecuritySessions",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_SecuritySessions_UserId",
                schema: "Identity",
                table: "SecuritySessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_CategoryId",
                schema: "Preferences",
                table: "Skills",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_Name",
                schema: "Preferences",
                table: "Skills",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkillsCategories_Name",
                schema: "Preferences",
                table: "SkillsCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolutionContentBlocks_SolutionId",
                schema: "QA",
                table: "SolutionContentBlocks",
                column: "SolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_AuthorId",
                schema: "QA",
                table: "Solutions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_ProblemId",
                schema: "QA",
                table: "Solutions",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_SolutionVotes_SolutionId",
                schema: "QA",
                table: "SolutionVotes",
                column: "SolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_SolutionVotes_SolutionId_VoterId",
                schema: "QA",
                table: "SolutionVotes",
                columns: new[] { "SolutionId", "VoterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolutionVotes_VoterId",
                schema: "QA",
                table: "SolutionVotes",
                column: "VoterId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                schema: "Preferences",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TopicInterest_TopicId",
                table: "TopicInterest",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_Name",
                schema: "Preferences",
                table: "Topics",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Universities_Name",
                schema: "Education",
                table: "Universities",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_UserArchives_PerformedById",
                schema: "Identity",
                table: "UserArchives",
                column: "PerformedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserArchives_TargetId",
                schema: "Identity",
                table: "UserArchives",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotificationPreferences_UserId",
                schema: "Identity",
                table: "UserNotificationPreferences",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "Identity",
                table: "Users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                schema: "Identity",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                schema: "Identity",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersSecuritySettings_RecoveryEmail",
                schema: "Identity",
                table: "UsersSecuritySettings",
                column: "RecoveryEmail",
                unique: true,
                filter: "[RecoveryEmail] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcademicRecords",
                schema: "Education");

            migrationBuilder.DropTable(
                name: "Blocks",
                schema: "Profiles");

            migrationBuilder.DropTable(
                name: "CommentMedias",
                schema: "ContentManagement");

            migrationBuilder.DropTable(
                name: "CommentReactions",
                schema: "ContentManagement");

            migrationBuilder.DropTable(
                name: "CommunityAuditLogs",
                schema: "Communities");

            migrationBuilder.DropTable(
                name: "CommunityInvitations",
                schema: "Communities");

            migrationBuilder.DropTable(
                name: "CommunityJoinRequests",
                schema: "Communities");

            migrationBuilder.DropTable(
                name: "CommunityMemberships",
                schema: "Communities");

            migrationBuilder.DropTable(
                name: "CommunityRules",
                schema: "Communities");

            migrationBuilder.DropTable(
                name: "CommunitySettings",
                schema: "Communities");

            migrationBuilder.DropTable(
                name: "CompanyAdministrators",
                schema: "Jobs");

            migrationBuilder.DropTable(
                name: "DiscussionsDI",
                schema: "QA");

            migrationBuilder.DropTable(
                name: "Follows",
                schema: "Profiles");

            migrationBuilder.DropTable(
                name: "IdentityArchives",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "JobApplications",
                schema: "Jobs");

            migrationBuilder.DropTable(
                name: "JobSkills",
                schema: "Jobs");

            migrationBuilder.DropTable(
                name: "Mutes",
                schema: "Profiles");

            migrationBuilder.DropTable(
                name: "Notifications",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "EventsHolder");

            migrationBuilder.DropTable(
                name: "PasswordArchives",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "PostMedias",
                schema: "ContentManagement");

            migrationBuilder.DropTable(
                name: "PostReactions",
                schema: "ContentManagement");

            migrationBuilder.DropTable(
                name: "PostTags",
                schema: "ContentManagement");

            migrationBuilder.DropTable(
                name: "PostTopics",
                schema: "ContentManagement");

            migrationBuilder.DropTable(
                name: "PostViews",
                schema: "ContentManagement");

            migrationBuilder.DropTable(
                name: "ProblemContentBlocks",
                schema: "QA");

            migrationBuilder.DropTable(
                name: "ProblemTags",
                schema: "QA");

            migrationBuilder.DropTable(
                name: "ProblemTopics",
                schema: "QA");

            migrationBuilder.DropTable(
                name: "ProblemViews",
                schema: "QA");

            migrationBuilder.DropTable(
                name: "ProblemVotes",
                schema: "QA");

            migrationBuilder.DropTable(
                name: "ProfileSkills",
                schema: "ProfileContext");

            migrationBuilder.DropTable(
                name: "ProfileTopics",
                schema: "ProfileContext");

            migrationBuilder.DropTable(
                name: "ProfileViews",
                schema: "Profiles");

            migrationBuilder.DropTable(
                name: "ProjectContributors",
                schema: "Projects");

            migrationBuilder.DropTable(
                name: "ProjectMedias",
                schema: "Projects");

            migrationBuilder.DropTable(
                name: "ProjectMilestones",
                schema: "Projects");

            migrationBuilder.DropTable(
                name: "ProjectRatings",
                schema: "Projects");

            migrationBuilder.DropTable(
                name: "ProjectSkills",
                schema: "Projects");

            migrationBuilder.DropTable(
                name: "ProjectTags",
                schema: "Projects");

            migrationBuilder.DropTable(
                name: "ProjectViews",
                schema: "Projects");

            migrationBuilder.DropTable(
                name: "RecoveryCodes",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "RefreshTokens",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "ReputationLedgers",
                schema: "Profiles");

            migrationBuilder.DropTable(
                name: "ResumeCertificates",
                schema: "Resumes");

            migrationBuilder.DropTable(
                name: "ResumeEducations",
                schema: "Resumes");

            migrationBuilder.DropTable(
                name: "ResumeExperiences",
                schema: "Resumes");

            migrationBuilder.DropTable(
                name: "ResumeLanguages",
                schema: "Resumes");

            migrationBuilder.DropTable(
                name: "ResumeProjects",
                schema: "Resumes");

            migrationBuilder.DropTable(
                name: "ResumeSkills",
                schema: "Resumes");

            migrationBuilder.DropTable(
                name: "SavedJobs",
                schema: "Jobs");

            migrationBuilder.DropTable(
                name: "SavedPosts",
                schema: "ContentManagement");

            migrationBuilder.DropTable(
                name: "SavedProblems",
                schema: "QA");

            migrationBuilder.DropTable(
                name: "SavedProfiles",
                schema: "Profiles");

            migrationBuilder.DropTable(
                name: "SavedProjects",
                schema: "Projects");

            migrationBuilder.DropTable(
                name: "SavedSolutions",
                schema: "QA");

            migrationBuilder.DropTable(
                name: "SolutionContentBlocks",
                schema: "QA");

            migrationBuilder.DropTable(
                name: "SolutionVotes",
                schema: "QA");

            migrationBuilder.DropTable(
                name: "TopicInterest");

            migrationBuilder.DropTable(
                name: "UserArchives",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserNotificationPreferences",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserPassKeys",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Universities",
                schema: "Education");

            migrationBuilder.DropTable(
                name: "Comments",
                schema: "ContentManagement");

            migrationBuilder.DropTable(
                name: "Skills",
                schema: "Preferences");

            migrationBuilder.DropTable(
                name: "Tags",
                schema: "Preferences");

            migrationBuilder.DropTable(
                name: "SecuritySessions",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Resumes",
                schema: "Resumes");

            migrationBuilder.DropTable(
                name: "Jobs",
                schema: "Jobs");

            migrationBuilder.DropTable(
                name: "Projects",
                schema: "Projects");

            migrationBuilder.DropTable(
                name: "Solutions",
                schema: "QA");

            migrationBuilder.DropTable(
                name: "Topics",
                schema: "Preferences");

            migrationBuilder.DropTable(
                name: "ContentManagement",
                schema: "ContentManagement");

            migrationBuilder.DropTable(
                name: "SkillsCategories",
                schema: "Preferences");

            migrationBuilder.DropTable(
                name: "Devices",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Companies",
                schema: "Jobs");

            migrationBuilder.DropTable(
                name: "Problems",
                schema: "QA");

            migrationBuilder.DropTable(
                name: "UsersSecuritySettings",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Communities",
                schema: "Communities");

            migrationBuilder.DropTable(
                name: "Profiles",
                schema: "Profiles");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "Identity");
        }
    }
}
