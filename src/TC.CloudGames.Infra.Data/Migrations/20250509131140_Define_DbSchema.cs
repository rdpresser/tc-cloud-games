using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TC.CloudGames.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class Define_DbSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "users",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "games",
                newName: "games",
                newSchema: "public");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "users",
                schema: "public",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "games",
                schema: "public",
                newName: "games");
        }
    }
}
