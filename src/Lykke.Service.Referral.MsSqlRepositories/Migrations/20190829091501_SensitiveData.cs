using Microsoft.EntityFrameworkCore.Migrations;

namespace Lykke.Service.Referral.MsSqlRepositories.Migrations
{
    public partial class SensitiveData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_referral_hotel_email",
                schema: "referral",
                table: "referral_hotel");

            migrationBuilder.DropColumn(
                name: "email",
                schema: "referral",
                table: "referral_lead");

            migrationBuilder.DropColumn(
                name: "first_name",
                schema: "referral",
                table: "referral_lead");

            migrationBuilder.DropColumn(
                name: "last_name",
                schema: "referral",
                table: "referral_lead");

            migrationBuilder.DropColumn(
                name: "note",
                schema: "referral",
                table: "referral_lead");

            migrationBuilder.DropColumn(
                name: "phone_number",
                schema: "referral",
                table: "referral_lead");

            migrationBuilder.DropColumn(
                name: "email",
                schema: "referral",
                table: "referral_hotel");

            migrationBuilder.AddColumn<string>(
                name: "email_hash",
                schema: "referral",
                table: "referral_lead",
                type: "char(64)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone_number_hash",
                schema: "referral",
                table: "referral_lead",
                type: "char(64)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "email_hash",
                schema: "referral",
                table: "referral_hotel",
                type: "char(64)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_referral_hotel_email_hash",
                schema: "referral",
                table: "referral_hotel",
                column: "email_hash");

            migrationBuilder.CreateIndex(
                name: "IX_property_purchase_refer_lead_id",
                schema: "referral",
                table: "property_purchase",
                column: "refer_lead_id");

            migrationBuilder.CreateIndex(
                name: "IX_offer_to_purchase_refer_id",
                schema: "referral",
                table: "offer_to_purchase",
                column: "refer_id");

            migrationBuilder.AddForeignKey(
                name: "FK_offer_to_purchase_referral_lead_refer_id",
                schema: "referral",
                table: "offer_to_purchase",
                column: "refer_id",
                principalSchema: "referral",
                principalTable: "referral_lead",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_property_purchase_referral_lead_refer_lead_id",
                schema: "referral",
                table: "property_purchase",
                column: "refer_lead_id",
                principalSchema: "referral",
                principalTable: "referral_lead",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_offer_to_purchase_referral_lead_refer_id",
                schema: "referral",
                table: "offer_to_purchase");

            migrationBuilder.DropForeignKey(
                name: "FK_property_purchase_referral_lead_refer_lead_id",
                schema: "referral",
                table: "property_purchase");

            migrationBuilder.DropIndex(
                name: "IX_referral_hotel_email_hash",
                schema: "referral",
                table: "referral_hotel");

            migrationBuilder.DropIndex(
                name: "IX_property_purchase_refer_lead_id",
                schema: "referral",
                table: "property_purchase");

            migrationBuilder.DropIndex(
                name: "IX_offer_to_purchase_refer_id",
                schema: "referral",
                table: "offer_to_purchase");

            migrationBuilder.DropColumn(
                name: "email_hash",
                schema: "referral",
                table: "referral_lead");

            migrationBuilder.DropColumn(
                name: "phone_number_hash",
                schema: "referral",
                table: "referral_lead");

            migrationBuilder.DropColumn(
                name: "email_hash",
                schema: "referral",
                table: "referral_hotel");

            migrationBuilder.AddColumn<string>(
                name: "email",
                schema: "referral",
                table: "referral_lead",
                type: "nvarchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "first_name",
                schema: "referral",
                table: "referral_lead",
                type: "nvarchar(200)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "last_name",
                schema: "referral",
                table: "referral_lead",
                type: "nvarchar(200)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "note",
                schema: "referral",
                table: "referral_lead",
                type: "nvarchar(2000)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone_number",
                schema: "referral",
                table: "referral_lead",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "email",
                schema: "referral",
                table: "referral_hotel",
                type: "nvarchar(100)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_referral_hotel_email",
                schema: "referral",
                table: "referral_hotel",
                column: "email");
        }
    }
}
