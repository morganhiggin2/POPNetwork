using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class ReportsFriendUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FriendUsersDynamicValues",
                columns: table => new
                {
                    friendUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    numberOfReports = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendUsersDynamicValues", x => x.friendUserId);
                    table.ForeignKey(
                        name: "FK_FriendUsersDynamicValues_FriendUsers_friendUserId",
                        column: x => x.friendUserId,
                        principalTable: "FriendUsers",
                        principalColumn: "ApplicationUserId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendUsersDynamicValues");
        }
    }
}
