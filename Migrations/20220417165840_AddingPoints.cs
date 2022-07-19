using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class AddingPoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "friendActivityPoints",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    friendActivityId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_friendActivityPoints", x => x.id);
                    table.ForeignKey(
                        name: "FK_friendActivityPoints_FriendActivities_friendActivityId",
                        column: x => x.friendActivityId,
                        principalTable: "FriendActivities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "friendUserPoints",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    friendUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_friendUserPoints", x => x.id);
                    table.ForeignKey(
                        name: "FK_friendUserPoints_FriendUsers_friendUserId",
                        column: x => x.friendUserId,
                        principalTable: "FriendUsers",
                        principalColumn: "ApplicationUserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_friendActivityPoints_friendActivityId",
                table: "friendActivityPoints",
                column: "friendActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_friendUserPoints_friendUserId",
                table: "friendUserPoints",
                column: "friendUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "friendActivityPoints");

            migrationBuilder.DropTable(
                name: "friendUserPoints");
        }
    }
}
