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
            migrationBuilder.CreateTable(
                name: "InterestCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkillsCategories",
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
                name: "Interests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Interests_InterestCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "InterestCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    PhoneNumber = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    FailedLoginAttempts = table.Column<int>(type: "int", nullable: false),
                    PreferredLanguage = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLogIn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastPasswordChange = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsTwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    IsBanned = table.Column<bool>(type: "bit", nullable: false),
                    IsSuspended = table.Column<bool>(type: "bit", nullable: false),
                    SuspendedUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SecurityCode = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    CodeCreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
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
                        principalTable: "SkillsCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Faculties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UniversityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faculties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Faculties_Universities_UniversityId",
                        column: x => x.UniversityId,
                        principalTable: "Universities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TopicInterests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TopicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicInterests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopicInterests_Interests_InterestId",
                        column: x => x.InterestId,
                        principalTable: "Interests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TopicInterests_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityArchives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserIdentifier = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityArchives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityArchives_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ManualRecoveryRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmitterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactEmail = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    ContactPhone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    ProvidedInfoJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReviewerNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManualRecoveryRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManualRecoveryRequests_Users_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ManualRecoveryRequests_Users_SubmitterId",
                        column: x => x.SubmitterId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
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
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PasswordArchives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HashedPassword = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordArchives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordArchives_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PendingUpdates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupportId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedInfo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    UpdateType = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendingUpdates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PendingUpdates_Users_SupportId",
                        column: x => x.SupportId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PendingUpdates_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupportTickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Attachments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportTickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportTickets_Users_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserArchives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PerformedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserArchives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserArchives_Users_PerformedBy",
                        column: x => x.PerformedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserArchives_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastSeenAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LogoutAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DurationMinutes = table.Column<int>(type: "int", nullable: true),
                    IpAddress = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false),
                    Device = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Browser = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacultyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UniversityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profiles_Faculties_FacultyId",
                        column: x => x.FacultyId,
                        principalTable: "Faculties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Profiles_Universities_UniversityId",
                        column: x => x.UniversityId,
                        principalTable: "Universities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Profiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VerificationCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PendingUpdateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    FailedAttempts = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerificationCodes_PendingUpdates_PendingUpdateId",
                        column: x => x.PendingUpdateId,
                        principalTable: "PendingUpdates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VerificationCodes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupportResponses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentResponseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Attachments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsFromSupport = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportResponses_SupportResponses_ParentResponseId",
                        column: x => x.ParentResponseId,
                        principalTable: "SupportResponses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupportResponses_SupportTickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "SupportTickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupportResponses_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Blocks",
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
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Blocks_Profiles_BlockerId",
                        column: x => x.BlockerId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Communities",
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
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Follows",
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
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Follows_Profiles_FollowingId",
                        column: x => x.FollowingId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterestRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubmitterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterestRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterestRequests_Profiles_SubmitterId",
                        column: x => x.SubmitterId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    MinSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    MaxSalary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyCode = table.Column<string>(type: "nchar(3)", fixedLength: true, maxLength: 3, nullable: false),
                    SalaryType = table.Column<int>(type: "int", nullable: false),
                    KeyResponsibilitiesText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jobs_Profiles_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Jobs_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProfileInterests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileInterests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileInterests_Interests_InterestId",
                        column: x => x.InterestId,
                        principalTable: "Interests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileInterests_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileSkills",
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
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileTopics",
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
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileTopics_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileViews",
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
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileViews_Profiles_ViewerId",
                        column: x => x.ViewerId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
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
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Profiles_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Resumes",
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
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SkillRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SubmitterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillRequests_Profiles_SubmitterId",
                        column: x => x.SubmitterId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UniversityRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmitterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReviewComment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniversityRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UniversityRequests_Profiles_SubmitterId",
                        column: x => x.SubmitterId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommunityAuditLogs",
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
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommunityAuditLogs_Profiles_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CommunityCreationRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmitterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RulesText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Policy = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReviewNotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityCreationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityCreationRequests_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CommunityCreationRequests_Profiles_SubmitterId",
                        column: x => x.SubmitterId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommunityInvitations",
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
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommunityInvitations_Profiles_InviteeId",
                        column: x => x.InviteeId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommunityInvitations_Profiles_InviterId",
                        column: x => x.InviterId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommunityJoinRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmitterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityJoinRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunityJoinRequests_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommunityJoinRequests_Profiles_SubmitterId",
                        column: x => x.SubmitterId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommunityMemberships",
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
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommunityMemberships_Profiles_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommunityRules",
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
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommunitySettings",
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
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPinned = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Communities_CommunityId",
                        column: x => x.CommunityId,
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Posts_Profiles_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Problems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommunityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReadmeContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                        principalTable: "Communities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Problems_Profiles_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProfileInterestRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JoinerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterestRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileInterestRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileInterestRequests_InterestRequests_InterestRequestId",
                        column: x => x.InterestRequestId,
                        principalTable: "InterestRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileInterestRequests_Profiles_JoinerId",
                        column: x => x.JoinerId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobSkills",
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
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectContributors",
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
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectContributors_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMedias",
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
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMilestones",
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
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectRatings",
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
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectRatings_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectSkills",
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
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTags",
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
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectViews",
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
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectViews_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobApplications",
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
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobApplications_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobApplications_Profiles_ApplicantId",
                        column: x => x.ApplicantId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobApplications_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JobApplications_Resumes_ResumeId",
                        column: x => x.ResumeId,
                        principalTable: "Resumes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ResumeCertificates",
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
                        principalTable: "Resumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResumeEducations",
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
                        principalTable: "Resumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResumeExperiences",
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
                        principalTable: "Resumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResumeLanguages",
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
                        principalTable: "Resumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResumeProjects",
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
                        principalTable: "Resumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResumeSkills",
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
                        principalTable: "Resumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileSkillRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JoinerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SkillRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileSkillRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileSkillRequests_Profiles_JoinerId",
                        column: x => x.JoinerId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileSkillRequests_SkillRequests_SkillRequestId",
                        column: x => x.SkillRequestId,
                        principalTable: "SkillRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacultyRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmitterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UniversityRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UniversityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReviewComment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacultyRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacultyRequests_Profiles_SubmitterId",
                        column: x => x.SubmitterId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacultyRequests_Universities_UniversityId",
                        column: x => x.UniversityId,
                        principalTable: "Universities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FacultyRequests_UniversityRequests_UniversityRequestId",
                        column: x => x.UniversityRequestId,
                        principalTable: "UniversityRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ProfileUniversityRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UniversityRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JoinerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileUniversityRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileUniversityRequests_Profiles_JoinerId",
                        column: x => x.JoinerId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfileUniversityRequests_UniversityRequests_UniversityRequestId",
                        column: x => x.UniversityRequestId,
                        principalTable: "UniversityRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
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
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Profiles_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostMedias",
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
                        name: "FK_PostMedias_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostReactions",
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
                        name: "FK_PostReactions_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostReactions_Profiles_ReactorId",
                        column: x => x.ReactorId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostTags",
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
                        name: "FK_PostTags_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostTopics",
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
                        name: "FK_PostTopics_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostTopics_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostViews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ViewerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeviceType = table.Column<int>(type: "int", nullable: true),
                    IpHash = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostViews_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostViews_Profiles_ViewerId",
                        column: x => x.ViewerId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProblemContentBlocks",
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
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProblemTags",
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
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProblemTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProblemTopics",
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
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProblemTopics_Topics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProblemViews",
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
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProblemViews_Profiles_ViewerId",
                        column: x => x.ViewerId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProblemVotes",
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
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProblemVotes_Profiles_VoterId",
                        column: x => x.VoterId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Solutions",
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
                        principalTable: "Problems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Solutions_Profiles_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProfileFacultyRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JoinerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FacultyRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileFacultyRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileFacultyRequests_FacultyRequests_FacultyRequestId",
                        column: x => x.FacultyRequestId,
                        principalTable: "FacultyRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileFacultyRequests_Profiles_JoinerId",
                        column: x => x.JoinerId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommentMedias",
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
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommentReactions",
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
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentReactions_Profiles_ReactorId",
                        column: x => x.ReactorId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Discussions",
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
                    table.PrimaryKey("PK_Discussions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Discussions_Discussions_ParentDiscussionId",
                        column: x => x.ParentDiscussionId,
                        principalTable: "Discussions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Discussions_Profiles_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Discussions_Solutions_SolutionId",
                        column: x => x.SolutionId,
                        principalTable: "Solutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolutionContentBlocks",
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
                        principalTable: "Solutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SolutionVotes",
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
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolutionVotes_Solutions_SolutionId",
                        column: x => x.SolutionId,
                        principalTable: "Solutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_BlockedId",
                table: "Blocks",
                column: "BlockedId");

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_BlockerId",
                table: "Blocks",
                column: "BlockerId");

            migrationBuilder.CreateIndex(
                name: "IX_Blocks_BlockerId_BlockedId",
                table: "Blocks",
                columns: new[] { "BlockerId", "BlockedId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentMedias_CommentId",
                table: "CommentMedias",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReactions_CommentId",
                table: "CommentReactions",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentReactions_CommentId_ReactorId",
                table: "CommentReactions",
                columns: new[] { "CommentId", "ReactorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentReactions_ReactorId",
                table: "CommentReactions",
                column: "ReactorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_AuthorId",
                table: "Comments",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                table: "Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_Name",
                table: "Communities",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_OwnerId",
                table: "Communities",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityAuditLogs_ActorId",
                table: "CommunityAuditLogs",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityAuditLogs_CommunityId",
                table: "CommunityAuditLogs",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityCreationRequests_CommunityId",
                table: "CommunityCreationRequests",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityCreationRequests_SubmitterId",
                table: "CommunityCreationRequests",
                column: "SubmitterId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityInvitations_CommunityId",
                table: "CommunityInvitations",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityInvitations_InviteeId",
                table: "CommunityInvitations",
                column: "InviteeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommunityInvitations_InviterId",
                table: "CommunityInvitations",
                column: "InviterId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityJoinRequests_CommunityId",
                table: "CommunityJoinRequests",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityJoinRequests_SubmitterId",
                table: "CommunityJoinRequests",
                column: "SubmitterId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityMemberships_CommunityId",
                table: "CommunityMemberships",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityMemberships_CommunityId_MemberId",
                table: "CommunityMemberships",
                columns: new[] { "CommunityId", "MemberId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommunityMemberships_MemberId",
                table: "CommunityMemberships",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunityRules_CommunityId",
                table: "CommunityRules",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunitySettings_CommunityId",
                table: "CommunitySettings",
                column: "CommunityId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Discussions_AuthorId",
                table: "Discussions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Discussions_ParentDiscussionId",
                table: "Discussions",
                column: "ParentDiscussionId");

            migrationBuilder.CreateIndex(
                name: "IX_Discussions_SolutionId",
                table: "Discussions",
                column: "SolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Faculties_UniversityId",
                table: "Faculties",
                column: "UniversityId");

            migrationBuilder.CreateIndex(
                name: "IX_Faculties_UniversityId_Name",
                table: "Faculties",
                columns: new[] { "UniversityId", "Name" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyRequests_SubmitterId",
                table: "FacultyRequests",
                column: "SubmitterId");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyRequests_UniversityId",
                table: "FacultyRequests",
                column: "UniversityId");

            migrationBuilder.CreateIndex(
                name: "IX_FacultyRequests_UniversityRequestId",
                table: "FacultyRequests",
                column: "UniversityRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Follows_FollowerId",
                table: "Follows",
                column: "FollowerId");

            migrationBuilder.CreateIndex(
                name: "IX_Follows_FollowerId_FollowingId",
                table: "Follows",
                columns: new[] { "FollowerId", "FollowingId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Follows_FollowingId",
                table: "Follows",
                column: "FollowingId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityArchives_UserId",
                table: "IdentityArchives",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InterestCategories_Name",
                table: "InterestCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterestRequests_SubmitterId",
                table: "InterestRequests",
                column: "SubmitterId");

            migrationBuilder.CreateIndex(
                name: "IX_Interests_CategoryId",
                table: "Interests",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Interests_Name",
                table: "Interests",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_ApplicantId",
                table: "JobApplications",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_JobId_ApplicantId",
                table: "JobApplications",
                columns: new[] { "JobId", "ApplicantId" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_ProfileId",
                table: "JobApplications",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_JobApplications_ResumeId",
                table: "JobApplications",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_OwnerId",
                table: "Jobs",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_ProfileId",
                table: "Jobs",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_Title",
                table: "Jobs",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_JobSkills_JobId_SkillId",
                table: "JobSkills",
                columns: new[] { "JobId", "SkillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobSkills_SkillId",
                table: "JobSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualRecoveryRequests_ReviewerId",
                table: "ManualRecoveryRequests",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_ManualRecoveryRequests_SubmitterId",
                table: "ManualRecoveryRequests",
                column: "SubmitterId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TargetId",
                table: "Notifications",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordArchives_UserId",
                table: "PasswordArchives",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PendingUpdates_SupportId",
                table: "PendingUpdates",
                column: "SupportId");

            migrationBuilder.CreateIndex(
                name: "IX_PendingUpdates_UpdatedInfo",
                table: "PendingUpdates",
                column: "UpdatedInfo");

            migrationBuilder.CreateIndex(
                name: "IX_PendingUpdates_UserId",
                table: "PendingUpdates",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PostMedias_PostId",
                table: "PostMedias",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReactions_PostId",
                table: "PostReactions",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostReactions_PostId_ReactorId",
                table: "PostReactions",
                columns: new[] { "PostId", "ReactorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostReactions_ReactorId",
                table: "PostReactions",
                column: "ReactorId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AuthorId",
                table: "Posts",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CommunityId",
                table: "Posts",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Title",
                table: "Posts",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_PostTags_PostId_TagId",
                table: "PostTags",
                columns: new[] { "PostId", "TagId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostTags_TagId",
                table: "PostTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_PostTopics_PostId_TopicId",
                table: "PostTopics",
                columns: new[] { "PostId", "TopicId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostTopics_TopicId",
                table: "PostTopics",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_PostViews_PostId",
                table: "PostViews",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostViews_PostId_ViewerId",
                table: "PostViews",
                columns: new[] { "PostId", "ViewerId" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_PostViews_ViewerId",
                table: "PostViews",
                column: "ViewerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemContentBlocks_ProblemId",
                table: "ProblemContentBlocks",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_AuthorId",
                table: "Problems",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_CommunityId",
                table: "Problems",
                column: "CommunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_Title",
                table: "Problems",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemTags_ProblemId",
                table: "ProblemTags",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemTags_ProblemId_TagId",
                table: "ProblemTags",
                columns: new[] { "ProblemId", "TagId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProblemTags_TagId",
                table: "ProblemTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemTopics_ProblemId_TopicId",
                table: "ProblemTopics",
                columns: new[] { "ProblemId", "TopicId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProblemTopics_TopicId",
                table: "ProblemTopics",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemViews_ProblemId",
                table: "ProblemViews",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemViews_ProblemId_ViewerId",
                table: "ProblemViews",
                columns: new[] { "ProblemId", "ViewerId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProblemViews_ViewerId",
                table: "ProblemViews",
                column: "ViewerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemVotes_ProblemId",
                table: "ProblemVotes",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProblemVotes_ProblemId_VoterId",
                table: "ProblemVotes",
                columns: new[] { "ProblemId", "VoterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProblemVotes_VoterId",
                table: "ProblemVotes",
                column: "VoterId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileFacultyRequests_FacultyRequestId",
                table: "ProfileFacultyRequests",
                column: "FacultyRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileFacultyRequests_JoinerId",
                table: "ProfileFacultyRequests",
                column: "JoinerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileFacultyRequests_JoinerId_FacultyRequestId",
                table: "ProfileFacultyRequests",
                columns: new[] { "JoinerId", "FacultyRequestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileInterestRequests_InterestRequestId",
                table: "ProfileInterestRequests",
                column: "InterestRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileInterestRequests_JoinerId",
                table: "ProfileInterestRequests",
                column: "JoinerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileInterestRequests_JoinerId_InterestRequestId",
                table: "ProfileInterestRequests",
                columns: new[] { "JoinerId", "InterestRequestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileInterests_InterestId",
                table: "ProfileInterests",
                column: "InterestId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileInterests_ProfileId",
                table: "ProfileInterests",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileInterests_ProfileId_InterestId",
                table: "ProfileInterests",
                columns: new[] { "ProfileId", "InterestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_FacultyId",
                table: "Profiles",
                column: "FacultyId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_FullName",
                table: "Profiles",
                column: "FullName");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_UniversityId",
                table: "Profiles",
                column: "UniversityId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_UserId",
                table: "Profiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSkillRequests_JoinerId",
                table: "ProfileSkillRequests",
                column: "JoinerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSkillRequests_JoinerId_SkillRequestId",
                table: "ProfileSkillRequests",
                columns: new[] { "JoinerId", "SkillRequestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSkillRequests_SkillRequestId",
                table: "ProfileSkillRequests",
                column: "SkillRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSkills_ProfileId",
                table: "ProfileSkills",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSkills_ProfileId_SkillId",
                table: "ProfileSkills",
                columns: new[] { "ProfileId", "SkillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSkills_SkillId",
                table: "ProfileSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileTopics_ProfileId",
                table: "ProfileTopics",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileTopics_ProfileId_TopicId",
                table: "ProfileTopics",
                columns: new[] { "ProfileId", "TopicId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileTopics_TopicId",
                table: "ProfileTopics",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileUniversityRequests_JoinerId",
                table: "ProfileUniversityRequests",
                column: "JoinerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileUniversityRequests_JoinerId_UniversityRequestId",
                table: "ProfileUniversityRequests",
                columns: new[] { "JoinerId", "UniversityRequestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileUniversityRequests_UniversityRequestId",
                table: "ProfileUniversityRequests",
                column: "UniversityRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileViews_ViewedId",
                table: "ProfileViews",
                column: "ViewedId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileViews_ViewedId_ViewerId",
                table: "ProfileViews",
                columns: new[] { "ViewedId", "ViewerId" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileViews_ViewerId",
                table: "ProfileViews",
                column: "ViewerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectContributors_ContributorId",
                table: "ProjectContributors",
                column: "ContributorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectContributors_ProjectId",
                table: "ProjectContributors",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectContributors_ProjectId_ContributorId",
                table: "ProjectContributors",
                columns: new[] { "ProjectId", "ContributorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMedias_ProjectId",
                table: "ProjectMedias",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectMilestones_ProjectId",
                table: "ProjectMilestones",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRatings_ProjectId",
                table: "ProjectRatings",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRatings_ProjectId_RaterId",
                table: "ProjectRatings",
                columns: new[] { "ProjectId", "RaterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRatings_RaterId",
                table: "ProjectRatings",
                column: "RaterId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OwnerId",
                table: "Projects",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Title",
                table: "Projects",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSkills_ProjectId",
                table: "ProjectSkills",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSkills_ProjectId_SkillId",
                table: "ProjectSkills",
                columns: new[] { "ProjectId", "SkillId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSkills_SkillId",
                table: "ProjectSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTags_ProjectId",
                table: "ProjectTags",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTags_ProjectId_TagId",
                table: "ProjectTags",
                columns: new[] { "ProjectId", "TagId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTags_TagId",
                table: "ProjectTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectViews_ProjectId",
                table: "ProjectViews",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectViews_ProjectId_ViewerId",
                table: "ProjectViews",
                columns: new[] { "ProjectId", "ViewerId" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectViews_ViewerId",
                table: "ProjectViews",
                column: "ViewerId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeCertificates_ResumeId",
                table: "ResumeCertificates",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeEducations_ResumeId",
                table: "ResumeEducations",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeExperiences_ResumeId",
                table: "ResumeExperiences",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeLanguages_ResumeId",
                table: "ResumeLanguages",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeLanguages_ResumeId_Language",
                table: "ResumeLanguages",
                columns: new[] { "ResumeId", "Language" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_OwnerId",
                table: "Resumes",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_Title",
                table: "Resumes",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeSkills_ResumeId",
                table: "ResumeSkills",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeSkills_ResumeId_SkillName",
                table: "ResumeSkills",
                columns: new[] { "ResumeId", "SkillName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkillRequests_SubmitterId",
                table: "SkillRequests",
                column: "SubmitterId");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_CategoryId",
                table: "Skills",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Skills_Name",
                table: "Skills",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkillsCategories_Name",
                table: "SkillsCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolutionContentBlocks_SolutionId",
                table: "SolutionContentBlocks",
                column: "SolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_AuthorId",
                table: "Solutions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_ProblemId",
                table: "Solutions",
                column: "ProblemId");

            migrationBuilder.CreateIndex(
                name: "IX_SolutionVotes_SolutionId",
                table: "SolutionVotes",
                column: "SolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_SolutionVotes_SolutionId_VoterId",
                table: "SolutionVotes",
                columns: new[] { "SolutionId", "VoterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SolutionVotes_VoterId",
                table: "SolutionVotes",
                column: "VoterId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportResponses_ParentResponseId",
                table: "SupportResponses",
                column: "ParentResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportResponses_SenderId",
                table: "SupportResponses",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportResponses_TicketId",
                table: "SupportResponses",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_ApplicantId",
                table: "SupportTickets",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportTickets_ClosedBy",
                table: "SupportTickets",
                column: "ClosedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TopicInterests_InterestId",
                table: "TopicInterests",
                column: "InterestId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicInterests_TopicId",
                table: "TopicInterests",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicInterests_TopicId_InterestId",
                table: "TopicInterests",
                columns: new[] { "TopicId", "InterestId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Topics_Name",
                table: "Topics",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Universities_Name",
                table: "Universities",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_UniversityRequests_SubmitterId",
                table: "UniversityRequests",
                column: "SubmitterId");

            migrationBuilder.CreateIndex(
                name: "IX_UserArchives_PerformedBy",
                table: "UserArchives",
                column: "PerformedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserArchives_UserId",
                table: "UserArchives",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_PhoneNumber",
                table: "Users",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationCodes_PendingUpdateId",
                table: "VerificationCodes",
                column: "PendingUpdateId",
                unique: true,
                filter: "[PendingUpdateId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationCodes_UserId",
                table: "VerificationCodes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blocks");

            migrationBuilder.DropTable(
                name: "CommentMedias");

            migrationBuilder.DropTable(
                name: "CommentReactions");

            migrationBuilder.DropTable(
                name: "CommunityAuditLogs");

            migrationBuilder.DropTable(
                name: "CommunityCreationRequests");

            migrationBuilder.DropTable(
                name: "CommunityInvitations");

            migrationBuilder.DropTable(
                name: "CommunityJoinRequests");

            migrationBuilder.DropTable(
                name: "CommunityMemberships");

            migrationBuilder.DropTable(
                name: "CommunityRules");

            migrationBuilder.DropTable(
                name: "CommunitySettings");

            migrationBuilder.DropTable(
                name: "Discussions");

            migrationBuilder.DropTable(
                name: "Follows");

            migrationBuilder.DropTable(
                name: "IdentityArchives");

            migrationBuilder.DropTable(
                name: "JobApplications");

            migrationBuilder.DropTable(
                name: "JobSkills");

            migrationBuilder.DropTable(
                name: "ManualRecoveryRequests");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PasswordArchives");

            migrationBuilder.DropTable(
                name: "PostMedias");

            migrationBuilder.DropTable(
                name: "PostReactions");

            migrationBuilder.DropTable(
                name: "PostTags");

            migrationBuilder.DropTable(
                name: "PostTopics");

            migrationBuilder.DropTable(
                name: "PostViews");

            migrationBuilder.DropTable(
                name: "ProblemContentBlocks");

            migrationBuilder.DropTable(
                name: "ProblemTags");

            migrationBuilder.DropTable(
                name: "ProblemTopics");

            migrationBuilder.DropTable(
                name: "ProblemViews");

            migrationBuilder.DropTable(
                name: "ProblemVotes");

            migrationBuilder.DropTable(
                name: "ProfileFacultyRequests");

            migrationBuilder.DropTable(
                name: "ProfileInterestRequests");

            migrationBuilder.DropTable(
                name: "ProfileInterests");

            migrationBuilder.DropTable(
                name: "ProfileSkillRequests");

            migrationBuilder.DropTable(
                name: "ProfileSkills");

            migrationBuilder.DropTable(
                name: "ProfileTopics");

            migrationBuilder.DropTable(
                name: "ProfileUniversityRequests");

            migrationBuilder.DropTable(
                name: "ProfileViews");

            migrationBuilder.DropTable(
                name: "ProjectContributors");

            migrationBuilder.DropTable(
                name: "ProjectMedias");

            migrationBuilder.DropTable(
                name: "ProjectMilestones");

            migrationBuilder.DropTable(
                name: "ProjectRatings");

            migrationBuilder.DropTable(
                name: "ProjectSkills");

            migrationBuilder.DropTable(
                name: "ProjectTags");

            migrationBuilder.DropTable(
                name: "ProjectViews");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "ResumeCertificates");

            migrationBuilder.DropTable(
                name: "ResumeEducations");

            migrationBuilder.DropTable(
                name: "ResumeExperiences");

            migrationBuilder.DropTable(
                name: "ResumeLanguages");

            migrationBuilder.DropTable(
                name: "ResumeProjects");

            migrationBuilder.DropTable(
                name: "ResumeSkills");

            migrationBuilder.DropTable(
                name: "SolutionContentBlocks");

            migrationBuilder.DropTable(
                name: "SolutionVotes");

            migrationBuilder.DropTable(
                name: "SupportResponses");

            migrationBuilder.DropTable(
                name: "TopicInterests");

            migrationBuilder.DropTable(
                name: "UserArchives");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "VerificationCodes");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "FacultyRequests");

            migrationBuilder.DropTable(
                name: "InterestRequests");

            migrationBuilder.DropTable(
                name: "SkillRequests");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Resumes");

            migrationBuilder.DropTable(
                name: "Solutions");

            migrationBuilder.DropTable(
                name: "SupportTickets");

            migrationBuilder.DropTable(
                name: "Interests");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropTable(
                name: "PendingUpdates");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "UniversityRequests");

            migrationBuilder.DropTable(
                name: "SkillsCategories");

            migrationBuilder.DropTable(
                name: "Problems");

            migrationBuilder.DropTable(
                name: "InterestCategories");

            migrationBuilder.DropTable(
                name: "Communities");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "Faculties");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Universities");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
