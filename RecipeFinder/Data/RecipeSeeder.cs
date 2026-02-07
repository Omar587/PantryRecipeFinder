using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using CsvHelper;
using RecipeFinder.Models;
using RecipeFinder.Models.Enums;

namespace RecipeFinder.Data;

/*
 *All parts of the code within this class is just a means of populating
 * the database with mock data
 * 
 */
public class RecipeSeeder
{
    public static async Task SeedRecipes(AppDbContext context)
    {
        // Check if already seeded
        if (context.Recipes.Any()) return;

        var json = await File.ReadAllTextAsync("./Data/recipes.json");
        
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        var wrapper = JsonSerializer.Deserialize<List<RecipeData>>(json, options);
        var data = wrapper[0];

        foreach (var recipeDto in data.Recipes)
        {
            var recipe = new Recipe
            {
                Name = recipeDto.Name,
                ImageUrl = recipeDto.ImageUrl,
                Cuisine = ParseEnum<Cuisine>(recipeDto.Cuisine),
                Difficulty = ParseEnum<Difficulty>(recipeDto.Difficulty),
                PrepTime = recipeDto.PrepTime,
                CookTime = recipeDto.CookTime,
                Servings = recipeDto.Servings,
                Rating = recipeDto.Rating,
                RatingCount = recipeDto.RatingCount,
                Dietary = new DietaryInfo
                {
                    Vegetarian = recipeDto.Dietary.Vegetarian,
                    Vegan = recipeDto.Dietary.Vegan,
                    GlutenFree = recipeDto.Dietary.GlutenFree,
                    DairyFree = recipeDto.Dietary.DairyFree
                },
                Ingredients = recipeDto.Ingredients.Select(i => new Ingredient
                {
                    Name = i.Name,
                    Amount = i.Amount,
                    Unit = i.Unit,
                    Category = i.Category
                }).ToList()
            };

            // Handle tags (create or find existing)
            foreach (var tagName in recipeDto.Tags)
            {
                var tag = context.Tags.FirstOrDefault(t => t.Name == tagName)
                    ?? new Tag { Name = tagName };
                recipe.Tags.Add(tag);
            }

            context.Recipes.Add(recipe);
        }

        await context.SaveChangesAsync();
    }

    private static T ParseEnum<T>(string value) where T : struct
    {
        // Handle special cases for enum mapping
        var normalized = value.Replace(" ", "").Replace("-", "");
        return Enum.Parse<T>(normalized, ignoreCase: true);
    }
    
    public void SeedInstructions(AppDbContext context)
    {
        bool instructionsExist = context.RecipeInstructions.Any();
            
        if (instructionsExist)
        {
            return;
        }

        string filePath = "Data/instructions.csv";

        StreamReader fileReader = new StreamReader(filePath);
        CsvReader csvReader = new CsvReader(fileReader, CultureInfo.InvariantCulture);

        csvReader.Read();
        csvReader.ReadHeader();

        List<RecipeInstructions> instructionsList = new List<RecipeInstructions>();

        while (csvReader.Read())
        {
            int instructionId = csvReader.GetField<int>(0);
            int recipeId = csvReader.GetField<int>(1);
            int stepNumber = csvReader.GetField<int>(2);
            string instructionText = csvReader.GetField<string>(3);

            RecipeInstructions newInstruction = new RecipeInstructions();
            newInstruction.Id = instructionId;
            newInstruction.RecipeId = recipeId;
            newInstruction.StepNumber = stepNumber;
            newInstruction.Instruction = instructionText;

            instructionsList.Add(newInstruction);
        }

        csvReader.Dispose();
        fileReader.Dispose();

        context.RecipeInstructions.AddRange(instructionsList);
        context.SaveChanges();
    }
}

// DTOs for deserialization
public class RecipeData
{
    [JsonPropertyName("recipes")]
    public List<RecipeDto> Recipes { get; set; }
}

public class RecipeDto
{
    [JsonPropertyName("_id")]
    public string Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; }
    
    [JsonPropertyName("cuisine")]
    public string Cuisine { get; set; }
    
    [JsonPropertyName("difficulty")]
    public string Difficulty { get; set; }
    
    [JsonPropertyName("prepTime")]
    public int PrepTime { get; set; }
    
    [JsonPropertyName("cookTime")]
    public int CookTime { get; set; }
    
    [JsonPropertyName("servings")]
    public int Servings { get; set; }
    
    [JsonPropertyName("rating")]
    public double Rating { get; set; }
    
    [JsonPropertyName("ratingCount")]
    public int RatingCount { get; set; }
    
    [JsonPropertyName("ingredients")]
    public List<IngredientDto> Ingredients { get; set; }
    
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; }
    
    [JsonPropertyName("dietary")]
    public DietaryDto Dietary { get; set; }
}

public class IngredientDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("amount")]
    public double Amount { get; set; }
    
    [JsonPropertyName("unit")]
    public string Unit { get; set; }
    
    [JsonPropertyName("category")]
    public string Category { get; set; }
}

public class DietaryDto
{
    [JsonPropertyName("vegetarian")]
    public bool Vegetarian { get; set; }
    
    [JsonPropertyName("vegan")]
    public bool Vegan { get; set; }
    
    [JsonPropertyName("glutenFree")]
    public bool GlutenFree { get; set; }
    
    [JsonPropertyName("dairyFree")]
    public bool DairyFree { get; set; }
}