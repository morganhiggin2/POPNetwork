using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class ShownAndActiveFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "lastActive",
                table: "FriendUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "shown",
                table: "FriendUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "numAdmins",
                table: "FriendActivitiesDynamicValues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "numParticipants",
                table: "FriendActivitiesDynamicValues",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "shown",
                table: "FriendActivities",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lastActive",
                table: "FriendUsers");

            migrationBuilder.DropColumn(
                name: "shown",
                table: "FriendUsers");

            migrationBuilder.DropColumn(
                name: "numAdmins",
                table: "FriendActivitiesDynamicValues");

            migrationBuilder.DropColumn(
                name: "numParticipants",
                table: "FriendActivitiesDynamicValues");

            migrationBuilder.DropColumn(
                name: "shown",
                table: "FriendActivities");
        }
    }
}
