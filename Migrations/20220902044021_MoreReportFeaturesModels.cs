using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class MoreReportFeaturesModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FriendUserFriendActivityBlocks",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    friendActivityId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    friendUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendUserFriendActivityBlocks", x => x.id);
                    table.ForeignKey(
                        name: "FK_FriendUserFriendActivityBlocks_FriendActivities_friendActivityId",
                        column: x => x.friendActivityId,
                        principalTable: "FriendActivities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FriendUserFriendActivityBlocks_FriendUsers_friendUserId",
                        column: x => x.friendUserId,
                        principalTable: "FriendUsers",
                        principalColumn: "ApplicationUserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FriendUserFriendActivityBlocks_friendActivityId_friendUserId",
                table: "FriendUserFriendActivityBlocks",
                columns: new[] { "friendActivityId", "friendUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_FriendUserFriendActivityBlocks_friendUserId",
                table: "FriendUserFriendActivityBlocks",
                column: "friendUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendUserFriendActivityBlocks");
        }
    }
}
