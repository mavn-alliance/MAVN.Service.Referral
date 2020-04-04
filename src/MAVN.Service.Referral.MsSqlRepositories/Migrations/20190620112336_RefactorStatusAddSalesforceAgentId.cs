using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.Referral.MsSqlRepositories.Migrations
{
    public partial class RefactorStatusAddSalesforceAgentId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "status",
                schema: "referral",
                table: "referral_lead",
                newName: "response_status");

            migrationBuilder.RenameColumn(
                name: "referrer_id",
                schema: "referral",
                table: "referral_lead",
                newName: "agent_id");

            migrationBuilder.AddColumn<string>(
                name: "salesforce_agent_id",
                schema: "referral",
                table: "referral_lead",
                type: "varchar(200)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "state",
                schema: "referral",
                table: "referral_lead",
                type: "varchar(64)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "salesforce_agent_id",
                schema: "referral",
                table: "referral_lead");

            migrationBuilder.DropColumn(
                name: "state",
                schema: "referral",
                table: "referral_lead");

            migrationBuilder.RenameColumn(
                name: "response_status",
                schema: "referral",
                table: "referral_lead",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "agent_id",
                schema: "referral",
                table: "referral_lead",
                newName: "referrer_id");
        }
    }
}
