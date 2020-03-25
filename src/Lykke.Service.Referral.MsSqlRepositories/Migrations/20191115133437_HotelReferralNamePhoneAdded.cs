using Microsoft.EntityFrameworkCore.Migrations;

namespace Lykke.Service.Referral.MsSqlRepositories.Migrations
{
    public partial class HotelReferralNamePhoneAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "name_hash",
                schema: "referral",
                table: "referral_hotel",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone_number_hash",
                schema: "referral",
                table: "referral_hotel",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "name_hash",
                schema: "referral",
                table: "referral_hotel");

            migrationBuilder.DropColumn(
                name: "phone_number_hash",
                schema: "referral",
                table: "referral_hotel");
        }
    }
}
