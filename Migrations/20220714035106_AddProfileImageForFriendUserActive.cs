using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class AddProfileImageForFriendUserActive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "profile_image_0_active",
                table: "FriendUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "profile_image_1_active",
                table: "FriendUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "profile_image_2_active",
                table: "FriendUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "profile_image_0_active",
                table: "FriendUsers");

            migrationBuilder.DropColumn(
                name: "profile_image_1_active",
                table: "FriendUsers");

            migrationBuilder.DropColumn(
                name: "profile_image_2_active",
                table: "FriendUsers");
        }
    }
}
