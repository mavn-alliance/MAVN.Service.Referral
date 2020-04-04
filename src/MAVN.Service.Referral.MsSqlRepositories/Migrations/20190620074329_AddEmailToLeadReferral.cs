using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.Referral.MsSqlRepositories.Migrations
{
    public partial class AddEmailToLeadReferral : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "note",
                schema: "referral",
                table: "referral_lead",
                type: "nvarchar(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(2000)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "email",
                schema: "referral",
                table: "referral_lead",
                type: "varchar(100)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email",
                schema: "referral",
                table: "referral_lead");

            migrationBuilder.AlterColumn<string>(
                name: "note",
                schema: "referral",
                table: "referral_lead",
                type: "varchar(2000)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldNullable: true);
        }
    }
}
