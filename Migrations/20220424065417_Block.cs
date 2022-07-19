using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class Block : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FriendActivityFriendUserBlocks",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    friendActivityId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    friendUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendActivityFriendUserBlocks", x => x.id);
                    table.ForeignKey(
                        name: "FK_FriendActivityFriendUserBlocks_FriendActivities_friendActivityId",
                        column: x => x.friendActivityId,
                        principalTable: "FriendActivities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FriendActivityFriendUserBlocks_FriendUsers_friendUserId",
                        column: x => x.friendUserId,
                        principalTable: "FriendUsers",
                        principalColumn: "ApplicationUserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FriendUserFriendUserBlocks",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    friendUserFirstId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    friendUserLastId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendUserFriendUserBlocks", x => x.id);
                    table.ForeignKey(
                        name: "FK_FriendUserFriendUserBlocks_FriendActivities_friendUserFirstId",
                        column: x => x.friendUserFirstId,
                        principalTable: "FriendActivities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FriendUserFriendUserBlocks_FriendUsers_friendUserLastId",
                        column: x => x.friendUserLastId,
                        principalTable: "FriendUsers",
                        principalColumn: "ApplicationUserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FriendActivityFriendUserBlocks_friendActivityId_friendUserId",
                table: "FriendActivityFriendUserBlocks",
                columns: new[] { "friendActivityId", "friendUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_FriendActivityFriendUserBlocks_friendUserId",
                table: "FriendActivityFriendUserBlocks",
                column: "friendUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendUserFriendUserBlocks_friendUserFirstId_friendUserLastId",
                table: "FriendUserFriendUserBlocks",
                columns: new[] { "friendUserFirstId", "friendUserLastId" });

            migrationBuilder.CreateIndex(
                name: "IX_FriendUserFriendUserBlocks_friendUserLastId",
                table: "FriendUserFriendUserBlocks",
                column: "friendUserLastId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendActivityFriendUserBlocks");

            migrationBuilder.DropTable(
                name: "FriendUserFriendUserBlocks");
        }
    }
}
