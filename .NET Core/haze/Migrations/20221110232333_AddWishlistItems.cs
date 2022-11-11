using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace haze.Migrations
{
    public partial class AddWishlistItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WishlistItem_Products_ProductId",
                table: "WishlistItem");

            migrationBuilder.DropForeignKey(
                name: "FK_WishlistItem_Users_UserId",
                table: "WishlistItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WishlistItem",
                table: "WishlistItem");

            migrationBuilder.RenameTable(
                name: "WishlistItem",
                newName: "WishlistItems");

            migrationBuilder.RenameIndex(
                name: "IX_WishlistItem_UserId",
                table: "WishlistItems",
                newName: "IX_WishlistItems_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_WishlistItem_ProductId",
                table: "WishlistItems",
                newName: "IX_WishlistItems_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WishlistItems",
                table: "WishlistItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistItems_Products_ProductId",
                table: "WishlistItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistItems_Users_UserId",
                table: "WishlistItems",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WishlistItems_Products_ProductId",
                table: "WishlistItems");

            migrationBuilder.DropForeignKey(
                name: "FK_WishlistItems_Users_UserId",
                table: "WishlistItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WishlistItems",
                table: "WishlistItems");

            migrationBuilder.RenameTable(
                name: "WishlistItems",
                newName: "WishlistItem");

            migrationBuilder.RenameIndex(
                name: "IX_WishlistItems_UserId",
                table: "WishlistItem",
                newName: "IX_WishlistItem_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_WishlistItems_ProductId",
                table: "WishlistItem",
                newName: "IX_WishlistItem_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WishlistItem",
                table: "WishlistItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistItem_Products_ProductId",
                table: "WishlistItem",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WishlistItem_Users_UserId",
                table: "WishlistItem",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
