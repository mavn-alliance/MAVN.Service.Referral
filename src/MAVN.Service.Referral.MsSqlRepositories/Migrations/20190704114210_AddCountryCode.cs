using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.Referral.MsSqlRepositories.Migrations
{
    public partial class AddCountryCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "country_code",
                schema: "referral",
                table: "referral_lead",
                type: "varchar(10)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "country_code",
                schema: "referral",
                table: "referral_lead");
        }
    }
}
