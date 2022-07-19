using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class ChangePointDescriptionToPointCaption : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "description",
                table: "friendUserPoints",
                newName: "caption");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "friendActivityPoints",
                newName: "caption");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "caption",
                table: "friendUserPoints",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "caption",
                table: "friendActivityPoints",
                newName: "description");
        }
    }
}
