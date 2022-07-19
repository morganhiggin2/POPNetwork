using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class ChangeDesciminator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_friendUserPoints_FriendUsers_friendUserId",
                table: "friendUserPoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_friendUserPoints",
                table: "friendUserPoints");

            migrationBuilder.RenameTable(
                name: "friendUserPoints",
                newName: "FriendUserPoints");

            migrationBuilder.RenameColumn(
                name: "InvitationType",
                table: "InvitationBases",
                newName: "descriminator");

            migrationBuilder.RenameIndex(
                name: "IX_friendUserPoints_friendUserId",
                table: "FriendUserPoints",
                newName: "IX_FriendUserPoints_friendUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FriendUserPoints",
                table: "FriendUserPoints",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendUserPoints_FriendUsers_friendUserId",
                table: "FriendUserPoints",
                column: "friendUserId",
                principalTable: "FriendUsers",
                principalColumn: "ApplicationUserId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FriendUserPoints_FriendUsers_friendUserId",
                table: "FriendUserPoints");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FriendUserPoints",
                table: "FriendUserPoints");

            migrationBuilder.RenameTable(
                name: "FriendUserPoints",
                newName: "friendUserPoints");

            migrationBuilder.RenameColumn(
                name: "descriminator",
                table: "InvitationBases",
                newName: "InvitationType");

            migrationBuilder.RenameIndex(
                name: "IX_FriendUserPoints_friendUserId",
                table: "friendUserPoints",
                newName: "IX_friendUserPoints_friendUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_friendUserPoints",
                table: "friendUserPoints",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_friendUserPoints_FriendUsers_friendUserId",
                table: "friendUserPoints",
                column: "friendUserId",
                principalTable: "FriendUsers",
                principalColumn: "ApplicationUserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
