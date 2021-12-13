using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.BonusClientContext.Postgres.Migrations
{
    public partial class Country : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                schema: "bonusprogram",
                table: "clientcontexts",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                schema: "bonusprogram",
                table: "clientcontexts");
        }
    }
}
