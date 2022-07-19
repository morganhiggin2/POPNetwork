using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class InviationLinkSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "inviteeId",
                table: "Invitations");

            migrationBuilder.DropColumn(
                name: "inviterId",
                table: "Invitations");

            migrationBuilder.CreateTable(
                name: "GivenInviations",
                columns: table => new
                {
                    invitationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    inviterId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GivenInviations", x => x.invitationId);
                    table.ForeignKey(
                        name: "FK_GivenInviations_FriendActivities_inviterId",
                        column: x => x.inviterId,
                        principalTable: "FriendActivities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GivenInviations_FriendUsers_inviterId",
                        column: x => x.inviterId,
                        principalTable: "FriendUsers",
                        principalColumn: "ApplicationUserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GivenInviations_Invitations_invitationId",
                        column: x => x.invitationId,
                        principalTable: "Invitations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReceivedInviations",
                columns: table => new
                {
                    invitationId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    inviteeId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceivedInviations", x => x.invitationId);
                    table.ForeignKey(
                        name: "FK_ReceivedInviations_FriendActivities_inviteeId",
                        column: x => x.inviteeId,
                        principalTable: "FriendActivities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReceivedInviations_FriendUsers_inviteeId",
                        column: x => x.inviteeId,
                        principalTable: "FriendUsers",
                        principalColumn: "ApplicationUserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReceivedInviations_Invitations_invitationId",
                        column: x => x.invitationId,
                        principalTable: "Invitations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GivenInviations_inviterId",
                table: "GivenInviations",
                column: "inviterId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceivedInviations_inviteeId",
                table: "ReceivedInviations",
                column: "inviteeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GivenInviations");

            migrationBuilder.DropTable(
                name: "ReceivedInviations");

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

            migrationBuilder.AddColumn<string>(
                name: "inviteeId",
                table: "Invitations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "inviterId",
                table: "Invitations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_FriendActivities_FriendActivityid1",
                table: "Invitations",
                column: "FriendActivityid1",
                principalTable: "FriendActivities",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_FriendUsers_FriendUserApplicationUserId",
                table: "Invitations",
                column: "FriendUserApplicationUserId",
                principalTable: "FriendUsers",
                principalColumn: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_FriendUsers_FriendUserApplicationUserId1",
                table: "Invitations",
                column: "FriendUserApplicationUserId1",
                principalTable: "FriendUsers",
                principalColumn: "ApplicationUserId");
        }
    }
}
