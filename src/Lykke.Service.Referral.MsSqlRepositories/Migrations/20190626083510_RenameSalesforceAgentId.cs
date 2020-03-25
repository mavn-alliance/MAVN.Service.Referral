using Microsoft.EntityFrameworkCore.Migrations;

namespace Lykke.Service.Referral.MsSqlRepositories.Migrations
{
    public partial class RenameSalesforceAgentId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "salesforce_agent_id",
                schema: "referral",
                table: "referral_lead",
                newName: "agent_salesforce_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "agent_salesforce_id",
                schema: "referral",
                table: "referral_lead",
                newName: "salesforce_agent_id");
        }
    }
}
