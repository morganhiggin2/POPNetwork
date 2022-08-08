using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class ChangeFirstCharacterFromLowerToUpperCaseOfFriendActvityPoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_friendActivityPoints_FriendActivities_friendActivityId",
                table: "friendActivityPoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_friendActivityPoints",
                table: "friendActivityPoints");

            migrationBuilder.RenameTable(
                name: "friendActivityPoints",
                newName: "FriendActivityPoints");

            migrationBuilder.RenameIndex(
                name: "IX_friendActivityPoints_friendActivityId",
                table: "FriendActivityPoints",
                newName: "IX_FriendActivityPoints_friendActivityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FriendActivityPoints",
                table: "FriendActivityPoints",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendActivityPoints_FriendActivities_friendActivityId",
                table: "FriendActivityPoints",
                column: "friendActivityId",
                principalTable: "FriendActivities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FriendActivityPoints_FriendActivities_friendActivityId",
                table: "FriendActivityPoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FriendActivityPoints",
                table: "FriendActivityPoints");

            migrationBuilder.RenameTable(
                name: "FriendActivityPoints",
                newName: "friendActivityPoints");

            migrationBuilder.RenameIndex(
                name: "IX_FriendActivityPoints_friendActivityId",
                table: "friendActivityPoints",
                newName: "IX_friendActivityPoints_friendActivityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_friendActivityPoints",
                table: "friendActivityPoints",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_friendActivityPoints_FriendActivities_friendActivityId",
                table: "friendActivityPoints",
                column: "friendActivityId",
                principalTable: "FriendActivities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
