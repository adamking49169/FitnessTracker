using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class additionModelChangesForAPINotWorking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Workouts");

            migrationBuilder.AddColumn<int>(
                name: "ExerciseId",
                table: "Workouts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ExerciseDto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseDto", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_ExerciseId",
                table: "Workouts",
                column: "ExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_ExerciseDto_ExerciseId",
                table: "Workouts",
                column: "ExerciseId",
                principalTable: "ExerciseDto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_ExerciseDto_ExerciseId",
                table: "Workouts");

            migrationBuilder.DropTable(
                name: "ExerciseDto");

            migrationBuilder.DropIndex(
                name: "IX_Workouts_ExerciseId",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "ExerciseId",
                table: "Workouts");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Workouts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
