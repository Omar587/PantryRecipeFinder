using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeFinder.Data;
using RecipeFinder.Models;

namespace RecipeFinder.Controllers;

[Authorize]
public class FavoritesController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<Customer> _userManager;

    public FavoritesController(AppDbContext db, UserManager<Customer> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(_userManager.GetUserId(User)!);

        var favorites = await _db.FavoriteRecipes
            .Where(f => f.CustomerId == userId)
            .Include(f => f.Recipe)
            .OrderByDescending(f => f.AddedAt)
            .Select(f => f.Recipe)
            .ToListAsync();

        return View(favorites);
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int recipeId)
    {
        var userId = int.Parse(_userManager.GetUserId(User)!);

        var favorite = await _db.FavoriteRecipes
            .FirstOrDefaultAsync(f => f.CustomerId == userId && f.RecipeId == recipeId);

        if (favorite != null)
        {
            _db.FavoriteRecipes.Remove(favorite);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}