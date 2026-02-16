using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RecipeFinder.Data;
using RecipeFinder.Models;
using RecipeFinder.Services;
using RecipeFinder.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace RecipeFinder.Controllers;

public class RecipeController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<Customer> _userManager;
  
    public RecipeController(AppDbContext context, UserManager<Customer> userManager)
    {
        _context = context;
        _userManager = userManager;
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
        var recipe = _context.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Tags)
            .Include(r => r.Ratings)
            .Include(r => r.Notes)
            .FirstOrDefault(r => r.Id == id);

        if (recipe == null)
            return NotFound();

        return View(recipe);
    }
    
    
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> SaveNote([FromBody] SaveNoteRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        // Check if note already exists
        var existingNote = await _context.RecipeNotes
            .FirstOrDefaultAsync(n => n.CustomerId == user.Id && n.RecipeId == request.RecipeId);

        if (existingNote != null)
        {
            // Update existing note
            existingNote.NoteText = request.NoteText;
            existingNote.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            // Create new note
            var newNote = new RecipeNote
            {
                CustomerId = user.Id,
                RecipeId = request.RecipeId,
                NoteText = request.NoteText,
                CreatedAt = DateTime.UtcNow
            };
            _context.RecipeNotes.Add(newNote);
        }

        await _context.SaveChangesAsync();

        return Json(new { 
            success = true, 
            createdAt = existingNote?.UpdatedAt ?? DateTime.UtcNow 
        });
    }

// Request model
    public class SaveNoteRequest
    {
        public int RecipeId { get; set; }
        public string NoteText { get; set; }
    }
    
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> DeleteNote([FromBody] DeleteNoteRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var note = await _context.RecipeNotes
            .FirstOrDefaultAsync(n => n.CustomerId == user.Id && n.RecipeId == request.RecipeId);

        if (note != null)
        {
            _context.RecipeNotes.Remove(note);
            await _context.SaveChangesAsync();
        }

        return Json(new { success = true });
    }

// Request model
    public class DeleteNoteRequest
    {
        public int RecipeId { get; set; }
    }

}