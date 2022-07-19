using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class RemovePendingMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FriendUserMessages_FriendUsers_FriendUserApplicationUserId",
                table: "FriendUserMessages");

            migrationBuilder.DropIndex(
                name: "IX_FriendUserMessages_FriendUserApplicationUserId",
                table: "FriendUserMessages");

            migrationBuilder.DropColumn(
                name: "FriendUserApplicationUserId",
                table: "FriendUserMessages");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FriendUserApplicationUserId",
                table: "FriendUserMessages",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FriendUserMessages_FriendUserApplicationUserId",
                table: "FriendUserMessages",
                column: "FriendUserApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendUserMessages_FriendUsers_FriendUserApplicationUserId",
                table: "FriendUserMessages",
                column: "FriendUserApplicationUserId",
                principalTable: "FriendUsers",
                principalColumn: "ApplicationUserId");
        }
    }
}
