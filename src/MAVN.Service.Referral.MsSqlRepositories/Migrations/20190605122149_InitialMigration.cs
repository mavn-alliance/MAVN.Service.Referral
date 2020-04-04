using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.Referral.MsSqlRepositories.Migrations
{
    public partial class InitialMigration : Migration
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
                    id = table.Column<Guid>(nullable: false),
                    referral_code = table.Column<string>(type: "varchar(64)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_customer_referral", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "friend_referral",
                schema: "referral",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    referrer_id = table.Column<Guid>(nullable: false),
                    referred_id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_friend_referral", x => x.id);
                    table.ForeignKey(
                        name: "FK_friend_referral_customer_referral_referred_id",
                        column: x => x.referred_id,
                        principalSchema: "referral",
                        principalTable: "customer_referral",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_friend_referral_customer_referral_referrer_id",
                        column: x => x.referrer_id,
                        principalSchema: "referral",
                        principalTable: "customer_referral",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
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
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_purchase_referral_customer_referral_referrer_id",
                        column: x => x.referrer_id,
                        principalSchema: "referral",
                        principalTable: "customer_referral",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_customer_referral_referral_code",
                schema: "referral",
                table: "customer_referral",
                column: "referral_code",
                unique: true,
                filter: "[referral_code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_friend_referral_referred_id",
                schema: "referral",
                table: "friend_referral",
                column: "referred_id");

            migrationBuilder.CreateIndex(
                name: "IX_friend_referral_referrer_id",
                schema: "referral",
                table: "friend_referral",
                column: "referrer_id");

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
                name: "customer_referral",
                schema: "referral");
        }
    }
}
