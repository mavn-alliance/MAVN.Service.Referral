using Microsoft.EntityFrameworkCore.Migrations;

namespace Lykke.Service.Referral.MsSqlRepositories.Migrations
{
    public partial class HotelReferralExtended : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "location",
                schema: "referral",
                table: "referral_hotel",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "partner_id",
                schema: "referral",
                table: "referral_hotel",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "location",
                schema: "referral",
                table: "referral_hotel");

            migrationBuilder.DropColumn(
                name: "partner_id",
                schema: "referral",
                table: "referral_hotel");
        }
    }
}
