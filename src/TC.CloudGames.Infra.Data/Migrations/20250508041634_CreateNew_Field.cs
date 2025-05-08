using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TC.CloudGames.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateNew_Field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_on_utc",
                table: "users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_on_utc",
                table: "games",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_on_utc",
                table: "users");

            migrationBuilder.DropColumn(
                name: "created_on_utc",
                table: "games");
        }
    }
}
