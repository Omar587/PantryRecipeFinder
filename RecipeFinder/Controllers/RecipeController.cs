using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using RecipeFinder.Models;
using RecipeFinder.Models.Enums;

namespace RecipeFinder.Controllers;

public class RecipeController : Controller
{
    private readonly List<Recipe> _recipes;

    public RecipeController()
    {
        // Load recipes from JSON once when the controller is created
        var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "recipes.json");
        var jsonString = System.IO.File.ReadAllText(jsonPath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };

        _recipes = JsonSerializer.Deserialize<List<Recipe>>(jsonString, options) ?? new List<Recipe>();
    }

    // GET: /Recipe
    public IActionResult Index()
    {
        return View(_recipes);
    }

    // GET: /Recipe/Details/155
    public IActionResult Details(int id)
    {
        var recipe = _recipes.FirstOrDefault(r => r.Id == id);
        if (recipe == null)
            return NotFound();

        return View(recipe);
    }
}