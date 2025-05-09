using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TC.CloudGames.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateGame_Unique_Index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_games_name",
                schema: "public",
                table: "games",
                column: "name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_games_name",
                schema: "public",
                table: "games");
        }
    }
}
