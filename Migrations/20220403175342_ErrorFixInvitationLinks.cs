using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class ErrorFixInvitationLinks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvitationBases_FriendActivities_FriendActivityToFriendUserInvitation_inviteeId",
                table: "InvitationBases");

            migrationBuilder.DropForeignKey(
                name: "FK_InvitationBases_FriendActivities_FriendUserToFriendActivityInvitation_inviterId",
                table: "InvitationBases");

            migrationBuilder.AlterColumn<string>(
                name: "FriendUserToFriendActivityInvitation_inviteeId",
                table: "InvitationBases",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FriendActivityToFriendUserInvitation_inviterId",
                table: "InvitationBases",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvitationBases_FriendActivityToFriendUserInvitation_inviterId",
                table: "InvitationBases",
                column: "FriendActivityToFriendUserInvitation_inviterId");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationBases_FriendUserToFriendActivityInvitation_inviteeId",
                table: "InvitationBases",
                column: "FriendUserToFriendActivityInvitation_inviteeId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvitationBases_FriendActivities_FriendActivityToFriendUserInvitation_inviterId",
                table: "InvitationBases",
                column: "FriendActivityToFriendUserInvitation_inviterId",
                principalTable: "FriendActivities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InvitationBases_FriendActivities_FriendUserToFriendActivityInvitation_inviteeId",
                table: "InvitationBases",
                column: "FriendUserToFriendActivityInvitation_inviteeId",
                principalTable: "FriendActivities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvitationBases_FriendActivities_FriendActivityToFriendUserInvitation_inviterId",
                table: "InvitationBases");

            migrationBuilder.DropForeignKey(
                name: "FK_InvitationBases_FriendActivities_FriendUserToFriendActivityInvitation_inviteeId",
                table: "InvitationBases");

            migrationBuilder.DropIndex(
                name: "IX_InvitationBases_FriendActivityToFriendUserInvitation_inviterId",
                table: "InvitationBases");

            migrationBuilder.DropIndex(
                name: "IX_InvitationBases_FriendUserToFriendActivityInvitation_inviteeId",
                table: "InvitationBases");

            migrationBuilder.AlterColumn<string>(
                name: "FriendUserToFriendActivityInvitation_inviteeId",
                table: "InvitationBases",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FriendActivityToFriendUserInvitation_inviterId",
                table: "InvitationBases",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InvitationBases_FriendActivities_FriendActivityToFriendUserInvitation_inviteeId",
                table: "InvitationBases",
                column: "FriendActivityToFriendUserInvitation_inviteeId",
                principalTable: "FriendActivities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InvitationBases_FriendActivities_FriendUserToFriendActivityInvitation_inviterId",
                table: "InvitationBases",
                column: "FriendUserToFriendActivityInvitation_inviterId",
                principalTable: "FriendActivities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
