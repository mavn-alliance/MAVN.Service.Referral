using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.Referral.MsSqlRepositories.Migrations
{
    public partial class RemoveSalesforceIdFromOfferToPurchase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "salesforce_id",
                schema: "referral",
                table: "property_purchase");

            migrationBuilder.DropColumn(
                name: "agent_salesforce_id",
                schema: "referral",
                table: "offer_to_purchase");

            migrationBuilder.DropColumn(
                name: "buyer_customer_id",
                schema: "referral",
                table: "offer_to_purchase");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "salesforce_id",
                schema: "referral",
                table: "property_purchase",
                type: "varchar(200)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "agent_salesforce_id",
                schema: "referral",
                table: "offer_to_purchase",
                type: "varchar(200)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "buyer_customer_id",
                schema: "referral",
                table: "offer_to_purchase",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
