using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class MessagesExpoTokensAndDirectMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "timeStamp",
                table: "InvitationBases",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "DirectMessages",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    senderUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    instanceCount = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectMessages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "FriendUserMessages",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    recieptId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    timeStamp = table.Column<long>(type: "bigint", nullable: false),
                    userExpoToken = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    messageType = table.Column<int>(type: "int", nullable: false),
                    messageId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FriendUserApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendUserMessages", x => x.id);
                    table.ForeignKey(
                        name: "FK_FriendUserMessages_FriendUsers_FriendUserApplicationUserId",
                        column: x => x.FriendUserApplicationUserId,
                        principalTable: "FriendUsers",
                        principalColumn: "ApplicationUserId");
                });

            migrationBuilder.CreateTable(
                name: "UserExpoTokens",
                columns: table => new
                {
                    expoToken = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    userId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExpoTokens", x => x.expoToken);
                    table.ForeignKey(
                        name: "FK_UserExpoTokens_AspNetUsers_userId",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FriendUserMessages_FriendUserApplicationUserId",
                table: "FriendUserMessages",
                column: "FriendUserApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendUserMessages_recieptId",
                table: "FriendUserMessages",
                column: "recieptId");

            migrationBuilder.CreateIndex(
                name: "IX_FriendUserMessages_userExpoToken",
                table: "FriendUserMessages",
                column: "userExpoToken");

            migrationBuilder.CreateIndex(
                name: "IX_UserExpoTokens_userId",
                table: "UserExpoTokens",
                column: "userId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DirectMessages");

            migrationBuilder.DropTable(
                name: "FriendUserMessages");

            migrationBuilder.DropTable(
                name: "UserExpoTokens");

            migrationBuilder.DropColumn(
                name: "timeStamp",
                table: "InvitationBases");
        }
    }
}
