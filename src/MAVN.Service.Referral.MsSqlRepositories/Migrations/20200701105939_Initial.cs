using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.Referral.MsSqlRepositories.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "referral");

            migrationBuilder.CreateTable(
                name: "customer_referral",
                schema: "referral",
                columns: table => new
                {
                    customer_id = table.Column<Guid>(nullable: false),
                    referral_code = table.Column<string>(type: "varchar(64)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_referral", x => x.customer_id);
                });

            migrationBuilder.CreateTable(
                name: "friend_referral",
                schema: "referral",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    referrer_id = table.Column<Guid>(nullable: false),
                    referred_id = table.Column<Guid>(nullable: true),
                    campaign_id = table.Column<Guid>(nullable: false),
                    state = table.Column<int>(nullable: false),
                    creation_datetime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_friend_referral", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "referral_hotel",
                schema: "referral",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    email_hash = table.Column<string>(type: "char(64)", nullable: true),
                    name_hash = table.Column<string>(type: "char(64)", nullable: true),
                    phone_number_hash = table.Column<string>(type: "char(64)", nullable: true),
                    phone_country_code_id = table.Column<int>(nullable: false),
                    referrer_id = table.Column<string>(nullable: true),
                    confirmation_token = table.Column<string>(nullable: true),
                    partner_id = table.Column<string>(nullable: true),
                    location = table.Column<string>(nullable: true),
                    campaign_id = table.Column<Guid>(nullable: true),
                    stake_enabled = table.Column<bool>(nullable: false),
                    state = table.Column<string>(nullable: false),
                    creation_datetime = table.Column<DateTime>(nullable: false),
                    expiration_datetime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_referral_hotel", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "purchase_referral",
                schema: "referral",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    referrer_id = table.Column<Guid>(nullable: false),
                    referred_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_purchase_referral", x => x.id);
                    table.ForeignKey(
                        name: "FK_purchase_referral_customer_referral_referred_id",
                        column: x => x.referred_id,
                        principalSchema: "referral",
                        principalTable: "customer_referral",
                        principalColumn: "customer_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_purchase_referral_customer_referral_referrer_id",
                        column: x => x.referrer_id,
                        principalSchema: "referral",
                        principalTable: "customer_referral",
                        principalColumn: "customer_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_customer_referral_referral_code",
                schema: "referral",
                table: "customer_referral",
                column: "referral_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_purchase_referral_referred_id",
                schema: "referral",
                table: "purchase_referral",
                column: "referred_id");

            migrationBuilder.CreateIndex(
                name: "IX_purchase_referral_referrer_id",
                schema: "referral",
                table: "purchase_referral",
                column: "referrer_id");

            migrationBuilder.CreateIndex(
                name: "IX_referral_hotel_confirmation_token",
                schema: "referral",
                table: "referral_hotel",
                column: "confirmation_token");

            migrationBuilder.CreateIndex(
                name: "IX_referral_hotel_email_hash",
                schema: "referral",
                table: "referral_hotel",
                column: "email_hash");

            migrationBuilder.CreateIndex(
                name: "IX_referral_hotel_referrer_id",
                schema: "referral",
                table: "referral_hotel",
                column: "referrer_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "friend_referral",
                schema: "referral");

            migrationBuilder.DropTable(
                name: "purchase_referral",
                schema: "referral");

            migrationBuilder.DropTable(
                name: "referral_hotel",
                schema: "referral");

            migrationBuilder.DropTable(
                name: "customer_referral",
                schema: "referral");
        }
    }
}
