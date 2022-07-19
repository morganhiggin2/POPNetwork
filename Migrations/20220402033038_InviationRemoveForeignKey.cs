using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class InviationRemoveForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_FriendActivities_inviteeId",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_FriendActivities_inviterId",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_FriendUsers_inviteeId",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_FriendUsers_inviterId",
                table: "Invitations");

            migrationBuilder.DropIndex(
                name: "IX_Invitations_inviteeId",
                table: "Invitations");

            migrationBuilder.DropIndex(
                name: "IX_Invitations_inviterId",
                table: "Invitations");

            migrationBuilder.AlterColumn<string>(
                name: "inviterId",
                table: "Invitations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "inviteeId",
                table: "Invitations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "FriendActivityid",
                table: "Invitations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FriendActivityid1",
                table: "Invitations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FriendUserApplicationUserId",
                table: "Invitations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FriendUserApplicationUserId1",
                table: "Invitations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_FriendActivityid",
                table: "Invitations",
                column: "FriendActivityid");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_FriendActivityid1",
                table: "Invitations",
                column: "FriendActivityid1");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_FriendUserApplicationUserId",
                table: "Invitations",
                column: "FriendUserApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_FriendUserApplicationUserId1",
                table: "Invitations",
                column: "FriendUserApplicationUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_FriendActivities_FriendActivityid",
                table: "Invitations",
                column: "FriendActivityid",
                principalTable: "FriendActivities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_FriendActivities_FriendActivityid1",
                table: "Invitations",
                column: "FriendActivityid1",
                principalTable: "FriendActivities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_FriendUsers_FriendUserApplicationUserId",
                table: "Invitations",
                column: "FriendUserApplicationUserId",
                principalTable: "FriendUsers",
                principalColumn: "ApplicationUserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_FriendUsers_FriendUserApplicationUserId1",
                table: "Invitations",
                column: "FriendUserApplicationUserId1",
                principalTable: "FriendUsers",
                principalColumn: "ApplicationUserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_FriendActivities_FriendActivityid",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_FriendActivities_FriendActivityid1",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_FriendUsers_FriendUserApplicationUserId",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_FriendUsers_FriendUserApplicationUserId1",
                table: "Invitations");

            migrationBuilder.DropIndex(
                name: "IX_Invitations_FriendActivityid",
                table: "Invitations");

            migrationBuilder.DropIndex(
                name: "IX_Invitations_FriendActivityid1",
                table: "Invitations");

            migrationBuilder.DropIndex(
                name: "IX_Invitations_FriendUserApplicationUserId",
                table: "Invitations");

            migrationBuilder.DropIndex(
                name: "IX_Invitations_FriendUserApplicationUserId1",
                table: "Invitations");

            migrationBuilder.DropColumn(
                name: "FriendActivityid",
                table: "Invitations");

            migrationBuilder.DropColumn(
                name: "FriendActivityid1",
                table: "Invitations");

            migrationBuilder.DropColumn(
                name: "FriendUserApplicationUserId",
                table: "Invitations");

            migrationBuilder.DropColumn(
                name: "FriendUserApplicationUserId1",
                table: "Invitations");

            migrationBuilder.AlterColumn<string>(
                name: "inviterId",
                table: "Invitations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "inviteeId",
                table: "Invitations",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_inviteeId",
                table: "Invitations",
                column: "inviteeId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_inviterId",
                table: "Invitations",
                column: "inviterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_FriendActivities_inviteeId",
                table: "Invitations",
                column: "inviteeId",
                principalTable: "FriendActivities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_FriendActivities_inviterId",
                table: "Invitations",
                column: "inviterId",
                principalTable: "FriendActivities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_FriendUsers_inviteeId",
                table: "Invitations",
                column: "inviteeId",
                principalTable: "FriendUsers",
                principalColumn: "ApplicationUserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_FriendUsers_inviterId",
                table: "Invitations",
                column: "inviterId",
                principalTable: "FriendUsers",
                principalColumn: "ApplicationUserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
