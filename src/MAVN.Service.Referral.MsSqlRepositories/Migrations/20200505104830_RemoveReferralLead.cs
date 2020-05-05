using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.Referral.MsSqlRepositories.Migrations
{
    public partial class RemoveReferralLead : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "offer_to_purchase",
                schema: "referral");

            migrationBuilder.DropTable(
                name: "property_purchase",
                schema: "referral");

            migrationBuilder.DropTable(
                name: "referral_lead",
                schema: "referral");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "referral_lead",
                schema: "referral",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    agent_id = table.Column<Guid>(nullable: false),
                    agent_salesforce_id = table.Column<string>(type: "varchar(200)", nullable: true),
                    campaign_id = table.Column<Guid>(nullable: true),
                    confirmation_token = table.Column<string>(type: "varchar(200)", nullable: true),
                    creation_datetime = table.Column<DateTime>(nullable: false),
                    email_hash = table.Column<string>(type: "char(64)", nullable: true),
                    phone_country_code_id = table.Column<int>(nullable: false),
                    phone_number_hash = table.Column<string>(type: "char(64)", nullable: true),
                    response_status = table.Column<string>(type: "varchar(64)", nullable: true),
                    salesforce_id = table.Column<string>(type: "varchar(200)", nullable: true),
                    stake_enabled = table.Column<bool>(nullable: false),
                    state = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_referral_lead", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "offer_to_purchase",
                schema: "referral",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    refer_id = table.Column<Guid>(nullable: false),
                    timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_offer_to_purchase", x => x.id);
                    table.ForeignKey(
                        name: "FK_offer_to_purchase_referral_lead_refer_id",
                        column: x => x.refer_id,
                        principalSchema: "referral",
                        principalTable: "referral_lead",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "property_purchase",
                schema: "referral",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    commission_number = table.Column<int>(nullable: false),
                    refer_lead_id = table.Column<Guid>(nullable: false),
                    timestamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_property_purchase", x => x.id);
                    table.ForeignKey(
                        name: "FK_property_purchase_referral_lead_refer_lead_id",
                        column: x => x.refer_lead_id,
                        principalSchema: "referral",
                        principalTable: "referral_lead",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_offer_to_purchase_refer_id",
                schema: "referral",
                table: "offer_to_purchase",
                column: "refer_id");

            migrationBuilder.CreateIndex(
                name: "IX_property_purchase_refer_lead_id",
                schema: "referral",
                table: "property_purchase",
                column: "refer_lead_id");
        }
    }
}
