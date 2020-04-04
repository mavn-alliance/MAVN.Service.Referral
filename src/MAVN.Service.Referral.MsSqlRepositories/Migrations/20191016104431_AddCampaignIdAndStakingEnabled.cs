using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.Referral.MsSqlRepositories.Migrations
{
    public partial class AddCampaignIdAndStakingEnabled : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "campaign_id",
                schema: "referral",
                table: "referral_lead",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "stake_enabled",
                schema: "referral",
                table: "referral_lead",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "campaign_id",
                schema: "referral",
                table: "referral_hotel",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "stake_enabled",
                schema: "referral",
                table: "referral_hotel",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "campaign_id",
                schema: "referral",
                table: "referral_lead");

            migrationBuilder.DropColumn(
                name: "stake_enabled",
                schema: "referral",
                table: "referral_lead");

            migrationBuilder.DropColumn(
                name: "campaign_id",
                schema: "referral",
                table: "referral_hotel");

            migrationBuilder.DropColumn(
                name: "stake_enabled",
                schema: "referral",
                table: "referral_hotel");
        }
    }
}
