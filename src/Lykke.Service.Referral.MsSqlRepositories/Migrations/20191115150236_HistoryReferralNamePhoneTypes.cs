using Microsoft.EntityFrameworkCore.Migrations;

namespace Lykke.Service.Referral.MsSqlRepositories.Migrations
{
    public partial class HistoryReferralNamePhoneTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "phone_number_hash",
                schema: "referral",
                table: "referral_hotel",
                type: "char(64)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name_hash",
                schema: "referral",
                table: "referral_hotel",
                type: "char(64)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "phone_number_hash",
                schema: "referral",
                table: "referral_hotel",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(64)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "name_hash",
                schema: "referral",
                table: "referral_hotel",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(64)",
                oldNullable: true);
        }
    }
}
