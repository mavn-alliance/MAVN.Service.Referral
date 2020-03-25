using Microsoft.EntityFrameworkCore.Migrations;

namespace Lykke.Service.Referral.MsSqlRepositories.Migrations
{
    public partial class AddCommisionNumberToPropertyPuchases : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "commission_number",
                schema: "referral",
                table: "property_purchase",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "commission_number",
                schema: "referral",
                table: "property_purchase");
        }
    }
}
