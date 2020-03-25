using Microsoft.EntityFrameworkCore.Migrations;

namespace Lykke.Service.Referral.MsSqlRepositories.Migrations
{
    public partial class AddPhoneCountryCodeAndName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "country_code",
                schema: "referral",
                table: "referral_lead");

            migrationBuilder.AddColumn<string>(
                name: "phone_country_code",
                schema: "referral",
                table: "referral_lead",
                type: "varchar(20)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone_country_name",
                schema: "referral",
                table: "referral_lead",
                type: "varchar(90)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "phone_country_code",
                schema: "referral",
                table: "referral_lead");

            migrationBuilder.DropColumn(
                name: "phone_country_name",
                schema: "referral",
                table: "referral_lead");

            migrationBuilder.AddColumn<string>(
                name: "country_code",
                schema: "referral",
                table: "referral_lead",
                type: "varchar(10)",
                nullable: true);
        }
    }
}
