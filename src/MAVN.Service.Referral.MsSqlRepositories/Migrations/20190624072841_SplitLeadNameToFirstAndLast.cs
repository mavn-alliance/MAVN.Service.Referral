using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.Referral.MsSqlRepositories.Migrations
{
    public partial class SplitLeadNameToFirstAndLast : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                schema: "referral",
                table: "referral_lead",
                newName: "last_name");

            migrationBuilder.AddColumn<string>(
                name: "first_name",
                schema: "referral",
                table: "referral_lead",
                type: "nvarchar(200)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "first_name",
                schema: "referral",
                table: "referral_lead");

            migrationBuilder.RenameColumn(
                name: "last_name",
                schema: "referral",
                table: "referral_lead",
                newName: "name");
        }
    }
}
