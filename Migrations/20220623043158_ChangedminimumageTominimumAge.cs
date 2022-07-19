using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class ChangedminimumageTominimumAge : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "minimumAge",
                table: "FriendActivities",
                newName: "minimumage");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "minimumage",
                table: "FriendActivities",
                newName: "minimumAge");
        }
    }
}
