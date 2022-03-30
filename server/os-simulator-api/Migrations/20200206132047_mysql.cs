using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SoMeSimulator.Migrations
{
    public partial class mysql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Sender = table.Column<string>(nullable: true),
                    Heading = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    Avatar = table.Column<string>(nullable: true),
                    MessageFlow = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Scenarios",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scenarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ScenarioId = table.Column<int>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    Props = table.Column<uint>(nullable: false),
                    Sender = table.Column<string>(nullable: true),
                    Avatar = table.Column<string>(nullable: true),
                    MessageFlow = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_Scenarios_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "Scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Phases",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    StartPercent = table.Column<double>(nullable: false),
                    ScenarioId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Phases_Scenarios_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "Scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UsrId = table.Column<int>(nullable: false),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UsrId",
                        column: x => x.UsrId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhasePost",
                columns: table => new
                {
                    PhaseId = table.Column<int>(nullable: false),
                    PostId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhasePost", x => new { x.PhaseId, x.PostId });
                    table.ForeignKey(
                        name: "FK_PhasePost_Phases_PhaseId",
                        column: x => x.PhaseId,
                        principalTable: "Phases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhasePost_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScenarioEvents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Sender = table.Column<string>(nullable: true),
                    Heading = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    PhaseId = table.Column<int>(nullable: false),
                    TimePercent = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScenarioEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ScenarioEvents_Phases_PhaseId",
                        column: x => x.PhaseId,
                        principalTable: "Phases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessionGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StressLevel = table.Column<uint>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Duration = table.Column<TimeSpan>(nullable: false),
                    ScenarioId = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: true),
                    StopDate = table.Column<DateTime>(nullable: true),
                    CurrentPhaseId = table.Column<int>(nullable: true),
                    TypeableCode = table.Column<string>(nullable: true),
                    PauseStart = table.Column<DateTime>(nullable: true),
                    PauseTimeSum = table.Column<TimeSpan>(nullable: false),
                    GroupName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionGroups_Phases_CurrentPhaseId",
                        column: x => x.CurrentPhaseId,
                        principalTable: "Phases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SessionGroups_Scenarios_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "Scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SessionGuid = table.Column<Guid>(nullable: false),
                    SessionGroupId = table.Column<int>(nullable: true),
                    Participant = table.Column<string>(nullable: true),
                    ScenarioId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Scenarios_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "Scenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sessions_SessionGroups_SessionGroupId",
                        column: x => x.SessionGroupId,
                        principalTable: "SessionGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SessionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Sender = table.Column<string>(nullable: true),
                    Heading = table.Column<string>(nullable: true),
                    Text = table.Column<string>(nullable: true),
                    Tag = table.Column<uint>(nullable: false),
                    BotReplyProperties = table.Column<uint>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    PostId = table.Column<int>(nullable: true),
                    CommentId = table.Column<int>(nullable: true),
                    ScenarioEventId = table.Column<int>(nullable: true),
                    MessageFlow = table.Column<uint>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    SendDateTime = table.Column<DateTime>(nullable: true),
                    SessionId = table.Column<int>(nullable: true),
                    ParentSessionLogId = table.Column<int>(nullable: true),
                    Avatar = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionLogs_SessionLogs_ParentSessionLogId",
                        column: x => x.ParentSessionLogId,
                        principalTable: "SessionLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SessionLogs_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ScenarioId",
                table: "Comment",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_PhasePost_PostId",
                table: "PhasePost",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Phases_ScenarioId",
                table: "Phases",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ScenarioEvents_PhaseId",
                table: "ScenarioEvents",
                column: "PhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionGroups_CurrentPhaseId",
                table: "SessionGroups",
                column: "CurrentPhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionGroups_ScenarioId",
                table: "SessionGroups",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionLogs_ParentSessionLogId",
                table: "SessionLogs",
                column: "ParentSessionLogId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionLogs_SessionId",
                table: "SessionLogs",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_ScenarioId",
                table: "Sessions",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_SessionGroupId",
                table: "Sessions",
                column: "SessionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UsrId",
                table: "UserRoles",
                column: "UsrId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "PhasePost");

            migrationBuilder.DropTable(
                name: "ScenarioEvents");

            migrationBuilder.DropTable(
                name: "SessionLogs");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "SessionGroups");

            migrationBuilder.DropTable(
                name: "Phases");

            migrationBuilder.DropTable(
                name: "Scenarios");
        }
    }
}
