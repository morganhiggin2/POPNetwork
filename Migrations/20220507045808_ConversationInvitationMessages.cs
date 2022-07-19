using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class ConversationInvitationMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConversationBases",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    descriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    conversationType = table.Column<int>(type: "int", nullable: false),
                    lastActive = table.Column<long>(type: "bigint", nullable: false),
                    friendActivityId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationBases", x => x.id);
                    table.ForeignKey(
                        name: "FK_ConversationBases_FriendActivities_friendActivityId",
                        column: x => x.friendActivityId,
                        principalTable: "FriendActivities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConversationMessages",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    senderUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    conversationId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    instanceCount = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationMessages", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConversationBases_friendActivityId",
                table: "ConversationBases",
                column: "friendActivityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConversationBases");

            migrationBuilder.DropTable(
                name: "ConversationMessages");
        }
    }
}
