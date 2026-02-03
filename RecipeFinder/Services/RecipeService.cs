using Microsoft.EntityFrameworkCore;
using RecipeFinder.Models;
using RecipeFinder.Models.Enums;

namespace RecipeFinder.Services;
using Data;

/*
 *CRUD operations on recipes belong here
 * 
 */
public class RecipeService
{
    private readonly AppDbContext _context;
    public RecipeService(AppDbContext context)
    {
        _context = context;
        
    }
    
    public Recipe? GetById(int id)
    {
        return _context.Recipes.Find(id);
    }

    public IEnumerable<Recipe> GetAll()
    {
        return _context.Recipes.ToList();
    }

    public void Add(Recipe recipe)
    {
        _context.Recipes.Add(recipe);
    }

    public void Update(Recipe recipe)
    {
        _context.Recipes.Update(recipe);
    }

    public void Delete(int id)
    {
        var recipe = _context.Recipes.Find(id);
        if (recipe != null)
        {
            _context.Recipes.Remove(recipe);
        }
    }

    public List<Recipe> FilterByCuisine(Cuisine cuisine)
    {
        return _context.Recipes
            .Where(r => r.Cuisine == cuisine)
            .ToList();
    }
    
    // Filter recipes by minimum rating (no need to include Dietary)
    public List<Recipe> FilterByRating(double rating)
    {
        var recipes = _context.Recipes
            .Where(r => r.Rating >= rating)
            .ToList();

        return recipes;
    }

    // Filter recipes by difficulty (no need to include Dietary)
    public List<Recipe> FilterByDifficulty(Difficulty difficulty)
    {
        var recipes = _context.Recipes
            .Where(r => r.Difficulty == difficulty)
            .ToList();

        return recipes;
    }

    // Filter recipes by cook time (no need to include Dietary)
    // cookTime parameter examples: "<30", "30-60", ">60"
    public List<Recipe> FilterByCookTime(string cookTime)
    {
        var query = _context.Recipes.AsQueryable();

        if (cookTime.StartsWith("<"))
        {
            if (int.TryParse(cookTime.Substring(1), out int max))
            {
                query = query.Where(r => r.CookTime < max);
            }
        }
        else if (cookTime.StartsWith(">"))
        {
            if (int.TryParse(cookTime.Substring(1), out int min))
            {
                query = query.Where(r => r.CookTime > min);
            }
        }
        else if (cookTime.Contains("-"))
        {
            var parts = cookTime.Split('-');
            if (int.TryParse(parts[0], out int min) && int.TryParse(parts[1], out int max))
            {
                query = query.Where(r => r.CookTime >= min && r.CookTime <= max);
            }
        }
        else
        {
            // invalid input, return empty
            return new List<Recipe>();
        }

        return query.ToList();
    }

    // Filter recipes by dietary option (include Dietary only here)
    public List<Recipe> FilterByDietaryInfo(string dietary)
    {
        var query = _context.Recipes
            .Include(r => r.Dietary) // needed here
            .AsQueryable();

        if (dietary.Equals("Vegetarian", StringComparison.OrdinalIgnoreCase))
            query = query.Where(r => r.Dietary != null && r.Dietary.Vegetarian);
        else if (dietary.Equals("Vegan", StringComparison.OrdinalIgnoreCase))
            query = query.Where(r => r.Dietary != null && r.Dietary.Vegan);
        else if (dietary.Equals("GlutenFree", StringComparison.OrdinalIgnoreCase))
            query = query.Where(r => r.Dietary != null && r.Dietary.GlutenFree);
        else if (dietary.Equals("DairyFree", StringComparison.OrdinalIgnoreCase))
            query = query.Where(r => r.Dietary != null && r.Dietary.DairyFree);
        else
            return query.ToList(); // unknown dietary option, return all

        return query.ToList();
    }

    public void TestIngridents()
    {

        var query = _context.Ingredients;
        Console.WriteLine(query.Where( id => id.RecipeId == 1));
        

    }

    public void Save()
    {
        _context.SaveChanges();
    }
    
}