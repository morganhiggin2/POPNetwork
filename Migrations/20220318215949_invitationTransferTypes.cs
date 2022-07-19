using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class invitationTransferTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "inviteeType",
                table: "Invitations");

            migrationBuilder.RenameColumn(
                name: "inviterType",
                table: "Invitations",
                newName: "transferType");

            migrationBuilder.AddColumn<int>(
                name: "invitationMethod",
                table: "FriendActivities",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "invitationMethod",
                table: "FriendActivities");

            migrationBuilder.RenameColumn(
                name: "transferType",
                table: "Invitations",
                newName: "inviterType");

            migrationBuilder.AddColumn<int>(
                name: "inviteeType",
                table: "Invitations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
