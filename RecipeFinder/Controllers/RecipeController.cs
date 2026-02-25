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
   
    
    
    public async Task<IActionResult> Details(int id)
    {
        var recipe = await _context.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Tags)
            .Include(r => r.Ratings)
            .Include(r => r.Notes)
            .Include(r => r.Instructions)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null)
            return NotFound();

        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.IsFavorited = await _context.FavoriteRecipes
                .AnyAsync(f => f.CustomerId == user.Id && f.RecipeId == id);
        }
        else
        {
            ViewBag.IsFavorited = false;
        }

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
    
    
    // POST: /Recipe/RateRecipe
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RateRecipe([FromBody] RateRecipeRequest request)
    {
        if (request.Rating < 1 || request.Rating > 5)
            return BadRequest(new { error = "Rating must be between 1 and 5." });

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        // get the user's rating
        var existingRating = await _context.RecipeRatings
            .FirstOrDefaultAsync(r => r.CustomerId == user.Id && r.RecipeId == request.RecipeId);

        if (existingRating != null)
        {
            existingRating.Rating = request.Rating;
            existingRating.RatedAt = DateTime.UtcNow;
        }
        else
        {
            _context.RecipeRatings.Add(new RecipeRating
            {
                CustomerId = user.Id,
                RecipeId = request.RecipeId,
                Rating = request.Rating,
                RatedAt = DateTime.UtcNow
            });
        }

        await _context.SaveChangesAsync();

        // Recalculate the recipe's aggregate rating
        var allRatings = await _context.RecipeRatings
            .Where(r => r.RecipeId == request.RecipeId)
            .ToListAsync();

        var recipe = await _context.Recipes.FindAsync(request.RecipeId);
        if (recipe != null)
        {
            recipe.RatingCount = allRatings.Count;
            recipe.Rating = allRatings.Average(r => r.Rating);
            await _context.SaveChangesAsync();
        }

        return Json(new
        {
            success = true,
            newRating = recipe?.Rating ?? 0,
            newRatingCount = recipe?.RatingCount ?? 0,
            userRating = request.Rating
        });
    }
    
    public class RateRecipeRequest
    {
        public int RecipeId { get; set; }
        public int Rating { get; set; }
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> RemoveRating([FromBody] RemoveRatingRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var recipe = await _context.Recipes.FindAsync(request.RecipeId);
        if (recipe == null) return NotFound();

        var existingRating = await _context.RecipeRatings
            .FirstOrDefaultAsync(r => r.CustomerId == user.Id && r.RecipeId == request.RecipeId);

        if (existingRating != null)
        {
            if (recipe.RatingCount > 1)
            {
                recipe.Rating = ((recipe.Rating * recipe.RatingCount) - existingRating.Rating) / (recipe.RatingCount - 1);
                recipe.RatingCount--;
            }
            else
            {
                recipe.Rating = 0;
                recipe.RatingCount = 0;
            }

            _context.RecipeRatings.Remove(existingRating);
            await _context.SaveChangesAsync();
        }

        return Json(new { success = true, newRating = recipe.Rating, newRatingCount = recipe.RatingCount });
    }

    public class RemoveRatingRequest { public int RecipeId { get; set; } }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ToggleFavorite([FromBody] ToggleFavoriteRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var existing = await _context.FavoriteRecipes
            .FirstOrDefaultAsync(f => f.CustomerId == user.Id && f.RecipeId == request.RecipeId);

        bool isFavorited;

        if (existing != null)
        {
            _context.FavoriteRecipes.Remove(existing);
            isFavorited = false;
        }
        else
        {
            _context.FavoriteRecipes.Add(new FavoriteRecipe
            {
                CustomerId = user.Id,
                RecipeId = request.RecipeId,
                AddedAt = DateTime.UtcNow
            });
            isFavorited = true;
        }

        await _context.SaveChangesAsync();
        return Ok(new { isFavorited });
    }

    public class ToggleFavoriteRequest
    {
        public int RecipeId { get; set; }
    }


}