using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class FixMistakeOnBlock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FriendUserFriendUserBlocks_FriendActivities_friendUserFirstId",
                table: "FriendUserFriendUserBlocks");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendUserFriendUserBlocks_FriendUsers_friendUserFirstId",
                table: "FriendUserFriendUserBlocks",
                column: "friendUserFirstId",
                principalTable: "FriendUsers",
                principalColumn: "ApplicationUserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FriendUserFriendUserBlocks_FriendUsers_friendUserFirstId",
                table: "FriendUserFriendUserBlocks");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendUserFriendUserBlocks_FriendActivities_friendUserFirstId",
                table: "FriendUserFriendUserBlocks",
                column: "friendUserFirstId",
                principalTable: "FriendActivities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
