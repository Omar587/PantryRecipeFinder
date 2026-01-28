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
    
    
    //There are several options, vegan vegetarian, gluten free, diary free
    public List<Recipe> FilterByDietaryInfo(string dietary)
    {
        var query = _context.Recipes
            .Include(r => r.Dietary)
            .AsQueryable(); // allows dynamic filtering

        if (dietary.Equals("Vegetarian", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(r =>  r.Dietary.Vegetarian);
        }
        else if (dietary.Equals("Vegan", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(r => r.Dietary.Vegan);
        }
        else if (dietary.Equals("GlutenFree", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(r => r.Dietary.GlutenFree);
        }
        else if (dietary.Equals("DairyFree", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(r => r.Dietary.DairyFree);
        }
        

        return query.ToList();
    }

    // Filter recipes by minimum rating
    public List<Recipe> FilterByRating(double rating)
    {
        var recipes = _context.Recipes
            .Include(r => r.Dietary)   // include related Dietary info
            .Where(r => r.Rating >= rating)
            .ToList();

        return recipes;
    }

    // Filter recipes by difficulty (Easy, Medium, Hard)
    public List<Recipe> FilterByDifficulty(Difficulty difficulty)
    {
        var recipes = _context.Recipes
            .Include(r => r.Dietary)
            .Where(r => r.Difficulty == difficulty)
            .ToList();

        return recipes;
    }

    // Filter recipes by cook time
    // cookTime parameter format examples: "<30", "30-60", ">60"
    public List<Recipe> FilterByCookTime(string cookTime)
    {
        var query = _context.Recipes
            .Include(r => r.Dietary)
            .AsQueryable();

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

    public void Save()
    {
        _context.SaveChanges();
    }
    
}