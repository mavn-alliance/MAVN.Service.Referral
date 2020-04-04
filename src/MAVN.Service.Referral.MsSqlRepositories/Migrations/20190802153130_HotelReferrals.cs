using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.Referral.MsSqlRepositories.Migrations
{
    public partial class HotelReferrals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "referral_hotel",
                schema: "referral",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    referrer_id = table.Column<string>(nullable: true),
                    confirmation_token = table.Column<string>(nullable: true),
                    state = table.Column<string>(nullable: false),
                    creation_datetime = table.Column<DateTime>(nullable: false),
                    expiration_datetime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_referral_hotel", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_referral_hotel_confirmation_token",
                schema: "referral",
                table: "referral_hotel",
                column: "confirmation_token");

            migrationBuilder.CreateIndex(
                name: "IX_referral_hotel_email",
                schema: "referral",
                table: "referral_hotel",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "IX_referral_hotel_referrer_id",
                schema: "referral",
                table: "referral_hotel",
                column: "referrer_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "referral_hotel",
                schema: "referral");
        }
    }
}
