using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Games.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedDateToYearPlatform : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReleaseDate",
                table: "Platforms");

            migrationBuilder.AddColumn<int>(
                name: "ReleaseYear",
                table: "Platforms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReleaseYear",
                table: "Platforms");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReleaseDate",
                table: "Platforms",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
