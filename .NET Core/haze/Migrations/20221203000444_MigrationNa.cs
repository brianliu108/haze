using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace haze.Migrations
{
    public partial class MigrationNa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "ProductUserReviews",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "ProductUserReviews");
        }
    }
}
