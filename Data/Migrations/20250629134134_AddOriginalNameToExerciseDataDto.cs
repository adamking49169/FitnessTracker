using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOriginalNameToExerciseDataDto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_ExerciseDto_ExerciseId",
                table: "Workouts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExerciseDto",
                table: "ExerciseDto");

            migrationBuilder.RenameTable(
                name: "ExerciseDto",
                newName: "Exercises");

            migrationBuilder.AddColumn<string>(
                name: "NameOriginal",
                table: "Exercises",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Exercises",
                table: "Exercises",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_Exercises_ExerciseId",
                table: "Workouts",
                column: "ExerciseId",
                principalTable: "Exercises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Workouts_Exercises_ExerciseId",
                table: "Workouts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Exercises",
                table: "Exercises");

            migrationBuilder.DropColumn(
                name: "NameOriginal",
                table: "Exercises");

            migrationBuilder.RenameTable(
                name: "Exercises",
                newName: "ExerciseDto");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExerciseDto",
                table: "ExerciseDto",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Workouts_ExerciseDto_ExerciseId",
                table: "Workouts",
                column: "ExerciseId",
                principalTable: "ExerciseDto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
