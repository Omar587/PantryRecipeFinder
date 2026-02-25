using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeFinder.Migrations
{
    /// <inheritdoc />
    public partial class AddRecipeInstructions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RecipeInstructions_RecipeId",
                table: "RecipeInstructions",
                column: "RecipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeInstructions_Recipes_RecipeId",
                table: "RecipeInstructions",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeInstructions_Recipes_RecipeId",
                table: "RecipeInstructions");

            migrationBuilder.DropIndex(
                name: "IX_RecipeInstructions_RecipeId",
                table: "RecipeInstructions");
        }
    }
}
