using Microsoft.EntityFrameworkCore.Migrations;

namespace SoMeSimulator.Migrations
{
    public partial class PhaseComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Scenarios_ScenarioId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_PhasePost_Phases_PhaseId",
                table: "PhasePost");

            migrationBuilder.DropForeignKey(
                name: "FK_PhasePost_Posts_PostId",
                table: "PhasePost");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhasePost",
                table: "PhasePost");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.RenameTable(
                name: "PhasePost",
                newName: "PhasePosts");

            migrationBuilder.RenameTable(
                name: "Comment",
                newName: "Comments");

            migrationBuilder.RenameIndex(
                name: "IX_PhasePost_PostId",
                table: "PhasePosts",
                newName: "IX_PhasePosts_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_Comment_ScenarioId",
                table: "Comments",
                newName: "IX_Comments_ScenarioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhasePosts",
                table: "PhasePosts",
                columns: new[] { "PhaseId", "PostId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "PhaseComments",
                columns: table => new
                {
                    PhaseId = table.Column<int>(nullable: false),
                    CommentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhaseComments", x => new { x.PhaseId, x.CommentId });
                    table.ForeignKey(
                        name: "FK_PhaseComments_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhaseComments_Phases_PhaseId",
                        column: x => x.PhaseId,
                        principalTable: "Phases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhaseComments_CommentId",
                table: "PhaseComments",
                column: "CommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Scenarios_ScenarioId",
                table: "Comments",
                column: "ScenarioId",
                principalTable: "Scenarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhasePosts_Phases_PhaseId",
                table: "PhasePosts",
                column: "PhaseId",
                principalTable: "Phases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhasePosts_Posts_PostId",
                table: "PhasePosts",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Scenarios_ScenarioId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_PhasePosts_Phases_PhaseId",
                table: "PhasePosts");

            migrationBuilder.DropForeignKey(
                name: "FK_PhasePosts_Posts_PostId",
                table: "PhasePosts");

            migrationBuilder.DropTable(
                name: "PhaseComments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhasePosts",
                table: "PhasePosts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.RenameTable(
                name: "PhasePosts",
                newName: "PhasePost");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "Comment");

            migrationBuilder.RenameIndex(
                name: "IX_PhasePosts_PostId",
                table: "PhasePost",
                newName: "IX_PhasePost_PostId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_ScenarioId",
                table: "Comment",
                newName: "IX_Comment_ScenarioId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhasePost",
                table: "PhasePost",
                columns: new[] { "PhaseId", "PostId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Scenarios_ScenarioId",
                table: "Comment",
                column: "ScenarioId",
                principalTable: "Scenarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PhasePost_Phases_PhaseId",
                table: "PhasePost",
                column: "PhaseId",
                principalTable: "Phases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhasePost_Posts_PostId",
                table: "PhasePost",
                column: "PostId",
                principalTable: "Posts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
