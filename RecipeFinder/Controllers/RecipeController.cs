using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeFinder.Data;

namespace RecipeFinder.Controllers;

public class RecipeController : Controller
{
    private readonly AppDbContext _context;

    public RecipeController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var recipes = _context.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Tags)
            .ToList();

        return View(recipes);
    }

    public IActionResult Details(int id)
    {
        var recipe = _context.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Tags)
            .Include(r => r.Dietary)
            .FirstOrDefault(r => r.Id == id);

        if (recipe == null) return NotFound();

        return View(recipe);
    }
}
