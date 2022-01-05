using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.BonusClientContext.Postgres.Migrations
{
    public partial class NewKyc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "KYCDone",
                schema: "bonusprogram",
                table: "clientcontexts",
                newName: "KycWithdrawalAllowed");

            migrationBuilder.AddColumn<bool>(
                name: "KycDepositAllowed",
                schema: "bonusprogram",
                table: "clientcontexts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "KycTradeAllowed",
                schema: "bonusprogram",
                table: "clientcontexts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KycDepositAllowed",
                schema: "bonusprogram",
                table: "clientcontexts");

            migrationBuilder.DropColumn(
                name: "KycTradeAllowed",
                schema: "bonusprogram",
                table: "clientcontexts");

            migrationBuilder.RenameColumn(
                name: "KycWithdrawalAllowed",
                schema: "bonusprogram",
                table: "clientcontexts",
                newName: "KYCDone");
        }
    }
}
