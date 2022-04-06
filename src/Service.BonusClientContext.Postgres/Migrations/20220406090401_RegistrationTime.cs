using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.BonusClientContext.Postgres.Migrations
{
    public partial class RegistrationTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationDate",
                schema: "bonusprogram",
                table: "clientcontexts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegistrationDate",
                schema: "bonusprogram",
                table: "clientcontexts");
        }
    }
}
