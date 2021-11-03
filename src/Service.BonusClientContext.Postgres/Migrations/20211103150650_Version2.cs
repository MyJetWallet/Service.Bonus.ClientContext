using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.BonusClientContext.Postgres.Migrations
{
    public partial class Version2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_contexts",
                schema: "bonusprogram",
                table: "contexts");

            migrationBuilder.RenameTable(
                name: "contexts",
                schema: "bonusprogram",
                newName: "clientcontexts",
                newSchema: "bonusprogram");

            migrationBuilder.RenameIndex(
                name: "IX_contexts_ReferrerClientId",
                schema: "bonusprogram",
                table: "clientcontexts",
                newName: "IX_clientcontexts_ReferrerClientId");

            migrationBuilder.RenameIndex(
                name: "IX_contexts_ClientId",
                schema: "bonusprogram",
                table: "clientcontexts",
                newName: "IX_clientcontexts_ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_clientcontexts",
                schema: "bonusprogram",
                table: "clientcontexts",
                column: "ClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_clientcontexts",
                schema: "bonusprogram",
                table: "clientcontexts");

            migrationBuilder.RenameTable(
                name: "clientcontexts",
                schema: "bonusprogram",
                newName: "contexts",
                newSchema: "bonusprogram");

            migrationBuilder.RenameIndex(
                name: "IX_clientcontexts_ReferrerClientId",
                schema: "bonusprogram",
                table: "contexts",
                newName: "IX_contexts_ReferrerClientId");

            migrationBuilder.RenameIndex(
                name: "IX_clientcontexts_ClientId",
                schema: "bonusprogram",
                table: "contexts",
                newName: "IX_contexts_ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_contexts",
                schema: "bonusprogram",
                table: "contexts",
                column: "ClientId");
        }
    }
}
