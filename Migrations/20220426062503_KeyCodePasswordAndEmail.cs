using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class KeyCodePasswordAndEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResetPasswordKeyCodes",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    applicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    keycode = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResetPasswordKeyCodes", x => x.id);
                    table.ForeignKey(
                        name: "FK_ResetPasswordKeyCodes_AspNetUsers_applicationUserId",
                        column: x => x.applicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VerifyEmailKeyCodes",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    applicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    keycode = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerifyEmailKeyCodes", x => x.id);
                    table.ForeignKey(
                        name: "FK_VerifyEmailKeyCodes_AspNetUsers_applicationUserId",
                        column: x => x.applicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResetPasswordKeyCodes_applicationUserId",
                table: "ResetPasswordKeyCodes",
                column: "applicationUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VerifyEmailKeyCodes_applicationUserId",
                table: "VerifyEmailKeyCodes",
                column: "applicationUserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResetPasswordKeyCodes");

            migrationBuilder.DropTable(
                name: "VerifyEmailKeyCodes");
        }
    }
}
