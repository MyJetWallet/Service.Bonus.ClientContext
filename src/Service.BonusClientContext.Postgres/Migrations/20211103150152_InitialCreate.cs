using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.BonusClientContext.Postgres.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "bonusprogram");

            migrationBuilder.CreateTable(
                name: "contexts",
                schema: "bonusprogram",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    LastRecord = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    KYCDone = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    HasReferrer = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    HasReferrals = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ReferrerClientId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contexts", x => x.ClientId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_contexts_ClientId",
                schema: "bonusprogram",
                table: "contexts",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_contexts_ReferrerClientId",
                schema: "bonusprogram",
                table: "contexts",
                column: "ReferrerClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contexts",
                schema: "bonusprogram");
        }
    }
}
