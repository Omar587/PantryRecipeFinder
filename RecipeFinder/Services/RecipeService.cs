using RecipeFinder.Models;

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

    public void Save()
    {
        _context.SaveChanges();
    }
    
}