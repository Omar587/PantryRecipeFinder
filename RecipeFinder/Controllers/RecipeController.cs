using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using RecipeFinder.Data;
using RecipeFinder.Models;
using RecipeFinder.Services;
using RecipeFinder.Models.Enums;

namespace RecipeFinder.Controllers;

public class RecipeController : Controller
{
    private readonly List<Recipe> _recipes;
    private readonly AppDbContext _context;
  
    // Add this constructor
    public RecipeController(AppDbContext context)
    {
        _context = context;
    }
    
    // GET: /Recipe
    public IActionResult Index()
    {
        RecipeService recipeService = new RecipeService(_context);
        return View( recipeService.GetAll().Take(10));
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