using Microsoft.EntityFrameworkCore.Migrations;

namespace Lykke.Service.Referral.MsSqlRepositories.Migrations
{
    public partial class RenameIdToCustomerIdInCustomerReferral : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                schema: "referral",
                table: "customer_referral",
                newName: "customer_id");

            migrationBuilder.AlterColumn<int>(
                name: "state",
                schema: "referral",
                table: "referral_lead",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                schema: "referral",
                table: "referral_lead",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                schema: "referral",
                table: "referral_lead",
                type: "nvarchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "customer_id",
                schema: "referral",
                table: "customer_referral",
                newName: "id");

            migrationBuilder.AlterColumn<string>(
                name: "state",
                schema: "referral",
                table: "referral_lead",
                type: "varchar(64)",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "name",
                schema: "referral",
                table: "referral_lead",
                type: "varchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "email",
                schema: "referral",
                table: "referral_lead",
                type: "varchar(100)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldNullable: true);
        }
    }
}
