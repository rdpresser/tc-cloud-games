using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TC.CloudGames.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class Create_DbSeed_Admin_and_User : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "public",
                table: "users",
                columns: new[] { "id", "created_on_utc", "email", "first_name", "last_name", "password", "role" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ADMIN@ADMIN.COM", "Admin", "User", "Admin@123", "Admin" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), "USER@USER.COM", "Regular", "User", "User@123", "User" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "public",
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                schema: "public",
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));
        }
    }
}
