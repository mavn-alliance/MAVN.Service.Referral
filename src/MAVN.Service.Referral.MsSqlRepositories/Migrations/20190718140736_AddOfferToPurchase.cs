using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.Referral.MsSqlRepositories.Migrations
{
    public partial class AddOfferToPurchase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "offer_to_purchase",
                schema: "referral",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    refer_id = table.Column<Guid>(nullable: false),
                    agent_salesforce_id = table.Column<string>(type: "varchar(200)", nullable: true),
                    timestamp = table.Column<DateTime>(nullable: false),
                    buyer_customer_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_offer_to_purchase", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "offer_to_purchase",
                schema: "referral");
        }
    }
}
