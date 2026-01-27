using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using RecipeFinder.Data;
using RecipeFinder.Models;
using RecipeFinder.Services;
using RecipeFinder.Models.Enums;

namespace RecipeFinder.Controllers;

public class RecipeController : Controller
{
  
    private readonly AppDbContext _context;
  
    // Add this constructor
    public RecipeController(AppDbContext context)
    {
        _context = context;
    }
    
    // GET: /Recipe
    public IActionResult Index(int p)
    {
        int pageSize = 12;
        RecipeService recipeService = new RecipeService(_context);
    
        var query = recipeService.GetAll();
        var totalRecipes = query.Count(); 
    
        var recipes = query.Skip((p - 1) * pageSize).Take(pageSize).ToList();

        // Calculate total pages dynamically
        ViewBag.CurrentPage = p;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecipes / pageSize);

        return View(recipes);
    }

    // GET: /Recipe/Details/155
    public IActionResult Details(int id)
    {
        RecipeService recipeService = new RecipeService(_context);
        var recipe = recipeService.GetById(id);
        if (recipe == null)
            return NotFound();

        return View(recipe);
    }
    
    
}