using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace haze.Migrations
{
    public partial class UpdatePaymentInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingAddress",
                table: "PaymentInfo");

            migrationBuilder.DropColumn(
                name: "ShippingAddress",
                table: "PaymentInfo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BillingAddress",
                table: "PaymentInfo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress",
                table: "PaymentInfo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
