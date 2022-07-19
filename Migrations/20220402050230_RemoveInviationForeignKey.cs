using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class RemoveInviationForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_GivenInviations_id",
                table: "Invitations");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_ReceivedInviations_id",
                table: "Invitations");

            migrationBuilder.AddForeignKey(
                name: "FK_GivenInviations_Invitations_invitationId",
                table: "GivenInviations",
                column: "invitationId",
                principalTable: "Invitations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReceivedInviations_Invitations_invitationId",
                table: "ReceivedInviations",
                column: "invitationId",
                principalTable: "Invitations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GivenInviations_Invitations_invitationId",
                table: "GivenInviations");

            migrationBuilder.DropForeignKey(
                name: "FK_ReceivedInviations_Invitations_invitationId",
                table: "ReceivedInviations");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_GivenInviations_id",
                table: "Invitations",
                column: "id",
                principalTable: "GivenInviations",
                principalColumn: "invitationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_ReceivedInviations_id",
                table: "Invitations",
                column: "id",
                principalTable: "ReceivedInviations",
                principalColumn: "invitationId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
