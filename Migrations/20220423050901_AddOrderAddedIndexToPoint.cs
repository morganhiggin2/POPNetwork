using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POPNetwork.Migrations
{
    public partial class AddOrderAddedIndexToPoint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "orderAddedIndex",
                table: "friendUserPoints",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "orderAddedIndex",
                table: "friendActivityPoints",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "orderAddedIndex",
                table: "friendUserPoints");

            migrationBuilder.DropColumn(
                name: "orderAddedIndex",
                table: "friendActivityPoints");
        }
    }
}
