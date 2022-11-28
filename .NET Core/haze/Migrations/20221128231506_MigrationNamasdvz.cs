using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace haze.Migrations
{
    public partial class MigrationNamasdvz : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WishlistItems_Users_UserId1",
                table: "WishlistItems");

            migrationBuilder.DropIndex(
                name: "IX_WishlistItems_UserId1",
                table: "WishlistItems");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "WishlistItems");

            migrationBuilder.CreateIndex(
                name: "IX_UserProducts_UserId",
                table: "UserProducts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProducts_Users_UserId",
                table: "UserProducts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProducts_Users_UserId",
                table: "UserProducts");

            migrationBuilder.DropIndex(
                name: "IX_UserProducts_UserId",
                table: "UserProducts");

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "WishlistItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WishlistItems_UserId1",
                table: "WishlistItems",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistItems_Users_UserId1",
                table: "WishlistItems",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
