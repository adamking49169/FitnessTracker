using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMacroEntry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MacroEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CarbsGrams = table.Column<int>(type: "int", nullable: false),
                    ProteinGrams = table.Column<int>(type: "int", nullable: false),
                    FatGrams = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MacroEntries", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MacroEntries");
        }
    }
}
