using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.BonusClientContext.Postgres.Migrations
{
    public partial class DateTimeUtc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastRecord",
                schema: "bonusprogram",
                table: "clientcontexts",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastRecord",
                schema: "bonusprogram",
                table: "clientcontexts",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }
    }
}
