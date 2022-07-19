using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class NewInvitationModelInheritance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvitationBases",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    transferType = table.Column<int>(type: "int", nullable: false),
                    InvitationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    inviterId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    inviteeId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvitationBases", x => x.id);
                    table.ForeignKey(
                        name: "FK_InvitationBases_FriendUsers_inviteeId",
                        column: x => x.inviteeId,
                        principalTable: "FriendUsers",
                        principalColumn: "ApplicationUserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvitationBases_FriendUsers_inviterId",
                        column: x => x.inviterId,
                        principalTable: "FriendUsers",
                        principalColumn: "ApplicationUserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvitationBases_inviteeId",
                table: "InvitationBases",
                column: "inviteeId");

            migrationBuilder.CreateIndex(
                name: "IX_InvitationBases_inviterId",
                table: "InvitationBases",
                column: "inviterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvitationBases");
        }
    }
}
