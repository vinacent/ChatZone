using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatZone.Migrations
{
    public partial class Update2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "GroupOwnerId",
                table: "Comments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Comments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_GroupOwnerId",
                table: "Comments",
                column: "GroupOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_GroupOwnerId",
                table: "Comments",
                column: "GroupOwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_GroupOwnerId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_GroupOwnerId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "GroupOwnerId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Comments");
        }
    }
}
