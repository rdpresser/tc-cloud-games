using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TC.CloudGames.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class Create_Game_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "games",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    release_date = table.Column<DateOnly>(type: "date", nullable: false),
                    age_rating = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    developer_info = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    developer_info_publisher = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    disk_size_in_gb = table.Column<decimal>(type: "numeric", nullable: false),
                    price_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    playtime_hours = table.Column<int>(type: "integer", nullable: true),
                    playtime_player_count = table.Column<int>(type: "integer", nullable: true),
                    game_details_genre = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    game_details_platform = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    game_details_tags = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    game_details_game_mode = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    game_details_distribution_format = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    game_details_available_languages = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    game_details_supports_dlcs = table.Column<bool>(type: "boolean", nullable: false),
                    system_requirements_minimum = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    system_requirements_recommended = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    rating = table.Column<decimal>(type: "numeric", nullable: true),
                    official_link = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    game_status = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_games", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "games");
        }
    }
}
