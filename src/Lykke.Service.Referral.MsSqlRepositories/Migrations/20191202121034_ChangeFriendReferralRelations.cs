using Microsoft.EntityFrameworkCore.Migrations;

namespace Lykke.Service.Referral.MsSqlRepositories.Migrations
{
    public partial class ChangeFriendReferralRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_friend_referral_customer_referral_referred_id",
                schema: "referral",
                table: "friend_referral");

            migrationBuilder.DropForeignKey(
                name: "FK_friend_referral_customer_referral_referrer_id",
                schema: "referral",
                table: "friend_referral");

            migrationBuilder.DropIndex(
                name: "IX_friend_referral_referred_id",
                schema: "referral",
                table: "friend_referral");

            migrationBuilder.DropIndex(
                name: "IX_friend_referral_referrer_id",
                schema: "referral",
                table: "friend_referral");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddForeignKey(
                name: "FK_friend_referral_customer_referral_referred_id",
                schema: "referral",
                table: "friend_referral",
                column: "referred_id",
                principalSchema: "referral",
                principalTable: "customer_referral",
                principalColumn: "customer_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_friend_referral_customer_referral_referrer_id",
                schema: "referral",
                table: "friend_referral",
                column: "referrer_id",
                principalSchema: "referral",
                principalTable: "customer_referral",
                principalColumn: "customer_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
