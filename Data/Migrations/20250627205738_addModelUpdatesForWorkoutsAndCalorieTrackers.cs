using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class addModelUpdatesForWorkoutsAndCalorieTrackers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CaloriesBurned",
                table: "Workouts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserWeightKg",
                table: "Workouts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CaloriesBurned",
                table: "Workouts");

            migrationBuilder.DropColumn(
                name: "UserWeightKg",
                table: "Workouts");
        }
    }
}
