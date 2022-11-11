using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace haze.Migrations
{
    public partial class MigrationName12331 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Events_EventId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Events_EventId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_EventId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Products_EventId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "EventUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "EventProducts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventUsers_EventId",
                table: "EventUsers",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventProducts_EventId",
                table: "EventProducts",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventProducts_Events_EventId",
                table: "EventProducts",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventUsers_Events_EventId",
                table: "EventUsers",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventProducts_Events_EventId",
                table: "EventProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_EventUsers_Events_EventId",
                table: "EventUsers");

            migrationBuilder.DropIndex(
                name: "IX_EventUsers_EventId",
                table: "EventUsers");

            migrationBuilder.DropIndex(
                name: "IX_EventProducts_EventId",
                table: "EventProducts");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "EventUsers");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "EventProducts");

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_EventId",
                table: "Users",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_EventId",
                table: "Products",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Events_EventId",
                table: "Products",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Events_EventId",
                table: "Users",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");
        }
    }
}
