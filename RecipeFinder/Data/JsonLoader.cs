using System.Text.Json;
using RecipeFinder.Data;
using RecipeFinder.Models;
using RecipeFinder.Models.Enums;

namespace RecipeFinder.Import;

public static class JsonLoader
{
    public static async Task LoadRecipesAsync(
        AppDbContext context,
        string path)
    {
        var json = await File.ReadAllTextAsync(path);

        var dtoList = JsonSerializer.Deserialize<List<RecipeJsonDto>>(json);
        if (dtoList == null || dtoList.Count == 0)
            return;

        // Cache existing tags to avoid duplicates
        var tagCache = context.Tags
            .ToDictionary(t => t.Name.ToLower());

        foreach (var dto in dtoList)
        {
            // Basic duplicate protection (no model changes)
            bool exists = context.Recipes.Any(r =>
                r.Name == dto.name &&
                r.Cuisine == ParseCuisine(dto.cuisine));

            if (exists)
                continue;

            var recipe = new Recipe
            {
                Name = dto.name,
                ImageUrl = dto.imageUrl,
                Cuisine = ParseCuisine(dto.cuisine),
                Difficulty = ParseDifficulty(dto.difficulty),
                PrepTime = dto.prepTime,
                CookTime = dto.cookTime,
                Servings = dto.servings,
                Rating = dto.rating,
                RatingCount = dto.ratingCount,

                Dietary = new DietaryInfo
                {
                    Vegetarian = dto.dietary.vegetarian,
                    Vegan = dto.dietary.vegan,
                    GlutenFree = dto.dietary.glutenFree,
                    DairyFree = dto.dietary.dairyFree
                },

                Ingredients = dto.ingredients.Select(i => new Ingredient
                {
                    Name = i.name,
                    Amount = i.amount,
                    Unit = i.unit,
                    Category = i.category
                }).ToList()
            };

            foreach (var tagName in dto.tags)
            {
                var key = tagName.ToLower();

                if (!tagCache.TryGetValue(key, out var tag))
                {
                    tag = new Tag { Name = tagName };
                    tagCache[key] = tag;
                    context.Tags.Add(tag);
                }

                recipe.Tags.Add(tag);
            }

            context.Recipes.Add(recipe);
        }

        await context.SaveChangesAsync();
    }

    private static Difficulty ParseDifficulty(string value) =>
        Enum.Parse<Difficulty>(value, ignoreCase: true);

    private static Cuisine ParseCuisine(string value) =>
        Enum.TryParse<Cuisine>(value, true, out var result)
            ? result
            : Cuisine.Other;

    // ============================
    // JSON DTOs (NESTED)
    // ============================

    private class RecipeJsonDto
    {
        public string _id { get; set; } = "";
        public string name { get; set; } = "";
        public string imageUrl { get; set; } = "";
        public string cuisine { get; set; } = "";
        public string difficulty { get; set; } = "";
        public int prepTime { get; set; }
        public int cookTime { get; set; }
        public int servings { get; set; }

        public List<IngredientJsonDto> ingredients { get; set; } = [];
        public List<string> tags { get; set; } = [];
        public DietaryJsonDto dietary { get; set; } = null!;

        public double rating { get; set; }
        public int ratingCount { get; set; }
    }

    private class IngredientJsonDto
    {
        public string name { get; set; } = "";
        public double amount { get; set; }
        public string unit { get; set; } = "";
        public string category { get; set; } = "";
    }

    private class DietaryJsonDto
    {
        public bool vegetarian { get; set; }
        public bool vegan { get; set; }
        public bool glutenFree { get; set; }
        public bool dairyFree { get; set; }
    }
}
