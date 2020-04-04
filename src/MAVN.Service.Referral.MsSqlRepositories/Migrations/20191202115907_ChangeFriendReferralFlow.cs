using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MAVN.Service.Referral.MsSqlRepositories.Migrations
{
    public partial class ChangeFriendReferralFlow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "referred_id",
                schema: "referral",
                table: "friend_referral",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<DateTime>(
                name: "creation_datetime",
                schema: "referral",
                table: "friend_referral",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "state",
                schema: "referral",
                table: "friend_referral",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "creation_datetime",
                schema: "referral",
                table: "friend_referral");

            migrationBuilder.DropColumn(
                name: "state",
                schema: "referral",
                table: "friend_referral");

            migrationBuilder.AlterColumn<Guid>(
                name: "referred_id",
                schema: "referral",
                table: "friend_referral",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);
        }
    }
}
