using Microsoft.EntityFrameworkCore.Migrations;

namespace SoMeSimulator.Migrations
{
    public partial class UsrSessionGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsrId",
                table: "SessionGroups",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SessionGroups_UsrId",
                table: "SessionGroups",
                column: "UsrId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SessionGroups_Users_UsrId",
                table: "SessionGroups",
                column: "UsrId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SessionGroups_Users_UsrId",
                table: "SessionGroups");

            migrationBuilder.DropIndex(
                name: "IX_SessionGroups_UsrId",
                table: "SessionGroups");

            migrationBuilder.DropColumn(
                name: "UsrId",
                table: "SessionGroups");
        }
    }
}
