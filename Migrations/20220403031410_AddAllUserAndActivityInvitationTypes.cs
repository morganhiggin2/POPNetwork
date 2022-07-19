using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class AddAllUserAndActivityInvitationTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GivenInviations");

            migrationBuilder.DropTable(
                name: "ReceivedInviations");

            migrationBuilder.DropTable(
                name: "Invitations");

            migrationBuilder.AddColumn<string>(
                name: "FriendActivityToFriendActivityInvitation_inviteeId",
                table: "InvitationBases",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FriendActivityToFriendActivityInvitation_inviterId",
                table: "InvitationBases",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FriendActivityToFriendUserInvitation_inviteeId",
                table: "InvitationBases",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FriendActivityToFriendUserInvitation_inviterId",
                table: "InvitationBases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FriendUserToFriendActivityInvitation_inviteeId",
                table: "InvitationBases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FriendUserToFriendActivityInvitation_inviterId",
                table: "InvitationBases",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InvitationBases_FriendActivityToFriendActivityInvitation_inviteeId",
                table: "InvitationBases",
                column: "FriendActivityToFriendActivityInvitation_inviteeId");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationBases_FriendActivityToFriendActivityInvitation_inviterId",
                table: "InvitationBases",
                column: "FriendActivityToFriendActivityInvitation_inviterId");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationBases_FriendActivityToFriendUserInvitation_inviteeId",
                table: "InvitationBases",
                column: "FriendActivityToFriendUserInvitation_inviteeId");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationBases_FriendUserToFriendActivityInvitation_inviterId",
                table: "InvitationBases",
                column: "FriendUserToFriendActivityInvitation_inviterId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvitationBases_FriendActivities_FriendActivityToFriendActivityInvitation_inviteeId",
                table: "InvitationBases",
                column: "FriendActivityToFriendActivityInvitation_inviteeId",
                principalTable: "FriendActivities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InvitationBases_FriendActivities_FriendActivityToFriendActivityInvitation_inviterId",
                table: "InvitationBases",
                column: "FriendActivityToFriendActivityInvitation_inviterId",
                principalTable: "FriendActivities",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

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

            migrationBuilder.AddForeignKey(
                name: "FK_InvitationBases_FriendUsers_FriendActivityToFriendUserInvitation_inviteeId",
                table: "InvitationBases",
                column: "FriendActivityToFriendUserInvitation_inviteeId",
                principalTable: "FriendUsers",
                principalColumn: "ApplicationUserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InvitationBases_FriendUsers_FriendUserToFriendActivityInvitation_inviterId",
                table: "InvitationBases",
                column: "FriendUserToFriendActivityInvitation_inviterId",
                principalTable: "FriendUsers",
                principalColumn: "ApplicationUserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvitationBases_FriendActivities_FriendActivityToFriendActivityInvitation_inviteeId",
                table: "InvitationBases");

            migrationBuilder.DropForeignKey(
                name: "FK_InvitationBases_FriendActivities_FriendActivityToFriendActivityInvitation_inviterId",
                table: "InvitationBases");

            migrationBuilder.DropForeignKey(
                name: "FK_InvitationBases_FriendActivities_FriendActivityToFriendUserInvitation_inviteeId",
                table: "InvitationBases");

            migrationBuilder.DropForeignKey(
                name: "FK_InvitationBases_FriendActivities_FriendUserToFriendActivityInvitation_inviterId",
                table: "InvitationBases");

            migrationBuilder.DropForeignKey(
                name: "FK_InvitationBases_FriendUsers_FriendActivityToFriendUserInvitation_inviteeId",
                table: "InvitationBases");

            migrationBuilder.DropForeignKey(
                name: "FK_InvitationBases_FriendUsers_FriendUserToFriendActivityInvitation_inviterId",
                table: "InvitationBases");

            migrationBuilder.DropIndex(
                name: "IX_InvitationBases_FriendActivityToFriendActivityInvitation_inviteeId",
                table: "InvitationBases");

            migrationBuilder.DropIndex(
                name: "IX_InvitationBases_FriendActivityToFriendActivityInvitation_inviterId",
                table: "InvitationBases");

            migrationBuilder.DropIndex(
                name: "IX_InvitationBases_FriendActivityToFriendUserInvitation_inviteeId",
                table: "InvitationBases");

            migrationBuilder.DropIndex(
                name: "IX_InvitationBases_FriendUserToFriendActivityInvitation_inviterId",
                table: "InvitationBases");

            migrationBuilder.DropColumn(
                name: "FriendActivityToFriendActivityInvitation_inviteeId",
                table: "InvitationBases");

            migrationBuilder.DropColumn(
                name: "FriendActivityToFriendActivityInvitation_inviterId",
                table: "InvitationBases");

            migrationBuilder.DropColumn(
                name: "FriendActivityToFriendUserInvitation_inviteeId",
                table: "InvitationBases");

            migrationBuilder.DropColumn(
                name: "FriendActivityToFriendUserInvitation_inviterId",
                table: "InvitationBases");

            migrationBuilder.DropColumn(
                name: "FriendUserToFriendActivityInvitation_inviteeId",
                table: "InvitationBases");

            migrationBuilder.DropColumn(
                name: "FriendUserToFriendActivityInvitation_inviterId",
                table: "InvitationBases");

            migrationBuilder.CreateTable(
                name: "Invitations",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    transferType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "GivenInviations",
                columns: table => new
                {
                    invitationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FriendActivityid = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FriendUserApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    inviterId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GivenInviations", x => x.invitationId);
                    table.ForeignKey(
                        name: "FK_GivenInviations_FriendActivities_FriendActivityid",
                        column: x => x.FriendActivityid,
                        principalTable: "FriendActivities",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_GivenInviations_FriendUsers_FriendUserApplicationUserId",
                        column: x => x.FriendUserApplicationUserId,
                        principalTable: "FriendUsers",
                        principalColumn: "ApplicationUserId");
                    table.ForeignKey(
                        name: "FK_GivenInviations_Invitations_invitationId",
                        column: x => x.invitationId,
                        principalTable: "Invitations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReceivedInviations",
                columns: table => new
                {
                    invitationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FriendActivityid = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FriendUserApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    inviteeId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceivedInviations", x => x.invitationId);
                    table.ForeignKey(
                        name: "FK_ReceivedInviations_FriendActivities_FriendActivityid",
                        column: x => x.FriendActivityid,
                        principalTable: "FriendActivities",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_ReceivedInviations_FriendUsers_FriendUserApplicationUserId",
                        column: x => x.FriendUserApplicationUserId,
                        principalTable: "FriendUsers",
                        principalColumn: "ApplicationUserId");
                    table.ForeignKey(
                        name: "FK_ReceivedInviations_Invitations_invitationId",
                        column: x => x.invitationId,
                        principalTable: "Invitations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GivenInviations_FriendActivityid",
                table: "GivenInviations",
                column: "FriendActivityid");

            migrationBuilder.CreateIndex(
                name: "IX_GivenInviations_FriendUserApplicationUserId",
                table: "GivenInviations",
                column: "FriendUserApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivedInviations_FriendActivityid",
                table: "ReceivedInviations",
                column: "FriendActivityid");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivedInviations_FriendUserApplicationUserId",
                table: "ReceivedInviations",
                column: "FriendUserApplicationUserId");
        }
    }
}
