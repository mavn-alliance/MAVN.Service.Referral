using Microsoft.EntityFrameworkCore.Migrations;

namespace Lykke.Service.Referral.MsSqlRepositories.Migrations
{
    public partial class HotelReferralPhoneCountryCodeId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "phone_country_code_id",
                schema: "referral",
                table: "referral_hotel",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "phone_country_code_id",
                schema: "referral",
                table: "referral_hotel");
        }
    }
}
