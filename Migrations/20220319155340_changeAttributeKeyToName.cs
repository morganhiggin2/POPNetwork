using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class changeAttributeKeyToName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FriendUserFriendUserAttribute_FriendUserAttributes_attributesid",
                table: "FriendUserFriendUserAttribute");

            migrationBuilder.DropColumn(
                name: "attribute",
                table: "FriendUserAttributes");

            migrationBuilder.RenameColumn(
                name: "attributesid",
                table: "FriendUserFriendUserAttribute",
                newName: "attributesname");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "FriendUserAttributes",
                newName: "name");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendUserFriendUserAttribute_FriendUserAttributes_attributesname",
                table: "FriendUserFriendUserAttribute",
                column: "attributesname",
                principalTable: "FriendUserAttributes",
                principalColumn: "name",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FriendUserFriendUserAttribute_FriendUserAttributes_attributesname",
                table: "FriendUserFriendUserAttribute");

            migrationBuilder.RenameColumn(
                name: "attributesname",
                table: "FriendUserFriendUserAttribute",
                newName: "attributesid");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "FriendUserAttributes",
                newName: "id");

            migrationBuilder.AddColumn<string>(
                name: "attribute",
                table: "FriendUserAttributes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_FriendUserFriendUserAttribute_FriendUserAttributes_attributesid",
                table: "FriendUserFriendUserAttribute",
                column: "attributesid",
                principalTable: "FriendUserAttributes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
