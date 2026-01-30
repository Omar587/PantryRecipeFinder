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
  
    public RecipeController(AppDbContext context)
    {
        _context = context;
    }
    
    // GET: /Recipe
    public IActionResult Index(
        int page = 1,
        string? cuisine = null,
        string? difficulty = null,
        double? minRating = null,
        string? cookTime = null,
        string? dietary = null)
    {
        int pageSize = 12;
        RecipeService recipeService = new RecipeService(_context);
    
        var query = recipeService.GetAll();

        // Apply filters
        if (!string.IsNullOrEmpty(cuisine) && Enum.TryParse<Cuisine>(cuisine, out var cuisineEnum))
        {
            query = recipeService.FilterByCuisine(cuisineEnum);
        }

        if (!string.IsNullOrEmpty(difficulty) && Enum.TryParse<Difficulty>(difficulty, out var difficultyEnum))
        {
            var filteredByCuisine = query.ToList();
            query = recipeService.FilterByDifficulty(difficultyEnum)
                .Where(r => filteredByCuisine.Any(fc => fc.Id == r.Id))
                .AsQueryable();
        }

        if (minRating.HasValue && minRating.Value > 0)
        {
            var filteredSoFar = query.ToList();
            query = recipeService.FilterByRating(minRating.Value)
                .Where(r => filteredSoFar.Any(fc => fc.Id == r.Id))
                .AsQueryable();
        }

        if (!string.IsNullOrEmpty(cookTime))
        {
            var filteredSoFar = query.ToList();
            query = recipeService.FilterByCookTime(cookTime)
                .Where(r => filteredSoFar.Any(fc => fc.Id == r.Id))
                .AsQueryable();
        }

        if (!string.IsNullOrEmpty(dietary))
        {
            var filteredSoFar = query.ToList();
            query = recipeService.FilterByDietaryInfo(dietary)
                .Where(r => filteredSoFar.Any(fc => fc.Id == r.Id))
                .AsQueryable();
        }

        var totalRecipes = query.Count(); 
        var recipes = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        // Calculate total pages dynamically
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecipes / pageSize);

        // Pass filter values back to view
        ViewBag.SelectedCuisine = cuisine;
        ViewBag.SelectedDifficulty = difficulty;
        ViewBag.SelectedMinRating = minRating;
        ViewBag.SelectedCookTime = cookTime;
        ViewBag.SelectedDietary = dietary;

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