using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RecipeFinder.Migrations
{
    /// <inheritdoc />
    public partial class SeedForumFlairs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ForumFlairs",
                columns: new[] { "Id", "ColorHex", "Name" },
                values: new object[,]
                {
                    { 1, "#e85d26", "🍳 General Cooking" },
                    { 2, "#2d7d46", "📖 Recipe Share" },
                    { 3, "#2563eb", "🙋 Recipe Requests" },
                    { 4, "#b5550a", "🔪 Tips & Techniques" },
                    { 5, "#16a34a", "🌱 Vegetarian & Vegan" },
                    { 6, "#7c3aed", "🌍 World Cuisines" },
                    { 7, "#db2777", "📸 Food Photography" },
                    { 8, "#0891b2", "💬 Site Feedback" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ForumFlairs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ForumFlairs",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ForumFlairs",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ForumFlairs",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ForumFlairs",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ForumFlairs",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ForumFlairs",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ForumFlairs",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
