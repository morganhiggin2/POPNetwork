using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class RemoveForeignKeyInvitationLinks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GivenInviations_FriendActivities_inviterId",
                table: "GivenInviations");

            migrationBuilder.DropForeignKey(
                name: "FK_GivenInviations_FriendUsers_inviterId",
                table: "GivenInviations");

            migrationBuilder.DropForeignKey(
                name: "FK_GivenInviations_Invitations_invitationId",
                table: "GivenInviations");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceivedInviations_FriendActivities_inviteeId",
                table: "ReceivedInviations");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceivedInviations_FriendUsers_inviteeId",
                table: "ReceivedInviations");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceivedInviations_Invitations_invitationId",
                table: "ReceivedInviations");

            migrationBuilder.DropIndex(
                name: "IX_ReceivedInviations_inviteeId",
                table: "ReceivedInviations");

            migrationBuilder.DropIndex(
                name: "IX_GivenInviations_inviterId",
                table: "GivenInviations");

            migrationBuilder.AlterColumn<string>(
                name: "inviteeId",
                table: "ReceivedInviations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "FriendActivityid",
                table: "ReceivedInviations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FriendUserApplicationUserId",
                table: "ReceivedInviations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "inviterId",
                table: "GivenInviations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "FriendActivityid",
                table: "GivenInviations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FriendUserApplicationUserId",
                table: "GivenInviations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceivedInviations_FriendActivityid",
                table: "ReceivedInviations",
                column: "FriendActivityid");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivedInviations_FriendUserApplicationUserId",
                table: "ReceivedInviations",
                column: "FriendUserApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GivenInviations_FriendActivityid",
                table: "GivenInviations",
                column: "FriendActivityid");

            migrationBuilder.CreateIndex(
                name: "IX_GivenInviations_FriendUserApplicationUserId",
                table: "GivenInviations",
                column: "FriendUserApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_GivenInviations_FriendActivities_FriendActivityid",
                table: "GivenInviations",
                column: "FriendActivityid",
                principalTable: "FriendActivities",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_GivenInviations_FriendUsers_FriendUserApplicationUserId",
                table: "GivenInviations",
                column: "FriendUserApplicationUserId",
                principalTable: "FriendUsers",
                principalColumn: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_GivenInviations_Invitations_invitationId",
                table: "GivenInviations",
                column: "invitationId",
                principalTable: "Invitations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivedInviations_FriendActivities_FriendActivityid",
                table: "ReceivedInviations",
                column: "FriendActivityid",
                principalTable: "FriendActivities",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivedInviations_FriendUsers_FriendUserApplicationUserId",
                table: "ReceivedInviations",
                column: "FriendUserApplicationUserId",
                principalTable: "FriendUsers",
                principalColumn: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivedInviations_Invitations_invitationId",
                table: "ReceivedInviations",
                column: "invitationId",
                principalTable: "Invitations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GivenInviations_FriendActivities_FriendActivityid",
                table: "GivenInviations");

            migrationBuilder.DropForeignKey(
                name: "FK_GivenInviations_FriendUsers_FriendUserApplicationUserId",
                table: "GivenInviations");

            migrationBuilder.DropForeignKey(
                name: "FK_GivenInviations_Invitations_invitationId",
                table: "GivenInviations");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceivedInviations_FriendActivities_FriendActivityid",
                table: "ReceivedInviations");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceivedInviations_FriendUsers_FriendUserApplicationUserId",
                table: "ReceivedInviations");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceivedInviations_Invitations_invitationId",
                table: "ReceivedInviations");

            migrationBuilder.DropIndex(
                name: "IX_ReceivedInviations_FriendActivityid",
                table: "ReceivedInviations");

            migrationBuilder.DropIndex(
                name: "IX_ReceivedInviations_FriendUserApplicationUserId",
                table: "ReceivedInviations");

            migrationBuilder.DropIndex(
                name: "IX_GivenInviations_FriendActivityid",
                table: "GivenInviations");

            migrationBuilder.DropIndex(
                name: "IX_GivenInviations_FriendUserApplicationUserId",
                table: "GivenInviations");

            migrationBuilder.DropColumn(
                name: "FriendActivityid",
                table: "ReceivedInviations");

            migrationBuilder.DropColumn(
                name: "FriendUserApplicationUserId",
                table: "ReceivedInviations");

            migrationBuilder.DropColumn(
                name: "FriendActivityid",
                table: "GivenInviations");

            migrationBuilder.DropColumn(
                name: "FriendUserApplicationUserId",
                table: "GivenInviations");

            migrationBuilder.AlterColumn<string>(
                name: "inviteeId",
                table: "ReceivedInviations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "inviterId",
                table: "GivenInviations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivedInviations_inviteeId",
                table: "ReceivedInviations",
                column: "inviteeId");

            migrationBuilder.CreateIndex(
                name: "IX_GivenInviations_inviterId",
                table: "GivenInviations",
                column: "inviterId");

            migrationBuilder.AddForeignKey(
                name: "FK_GivenInviations_FriendActivities_inviterId",
                table: "GivenInviations",
                column: "inviterId",
                principalTable: "FriendActivities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GivenInviations_FriendUsers_inviterId",
                table: "GivenInviations",
                column: "inviterId",
                principalTable: "FriendUsers",
                principalColumn: "ApplicationUserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GivenInviations_Invitations_invitationId",
                table: "GivenInviations",
                column: "invitationId",
                principalTable: "Invitations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivedInviations_FriendActivities_inviteeId",
                table: "ReceivedInviations",
                column: "inviteeId",
                principalTable: "FriendActivities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivedInviations_FriendUsers_inviteeId",
                table: "ReceivedInviations",
                column: "inviteeId",
                principalTable: "FriendUsers",
                principalColumn: "ApplicationUserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivedInviations_Invitations_invitationId",
                table: "ReceivedInviations",
                column: "invitationId",
                principalTable: "Invitations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
