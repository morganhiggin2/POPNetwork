using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class ActivityAttributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "FriendActivities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "FriendActivityAttributes",
                columns: table => new
                {
                    name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendActivityAttributes", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "FriendActivityFriendActivityAttribute",
                columns: table => new
                {
                    activitiesid = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    attributesname = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendActivityFriendActivityAttribute", x => new { x.activitiesid, x.attributesname });
                    table.ForeignKey(
                        name: "FK_FriendActivityFriendActivityAttribute_FriendActivities_activitiesid",
                        column: x => x.activitiesid,
                        principalTable: "FriendActivities",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FriendActivityFriendActivityAttribute_FriendActivityAttributes_attributesname",
                        column: x => x.attributesname,
                        principalTable: "FriendActivityAttributes",
                        principalColumn: "name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FriendActivityFriendActivityAttribute_attributesname",
                table: "FriendActivityFriendActivityAttribute",
                column: "attributesname");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FriendActivityFriendActivityAttribute");

            migrationBuilder.DropTable(
                name: "FriendActivityAttributes");

            migrationBuilder.DropColumn(
                name: "description",
                table: "FriendActivities");
        }
    }
}
