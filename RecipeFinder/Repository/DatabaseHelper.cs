using RecipeFinder.Data;
using RecipeFinder.Models;

namespace RecipeFinder.Repository;

public class DatabaseHelper
{
    private readonly AppDbContext _context;

    public DatabaseHelper(AppDbContext context)
    {
        _context = context;
    }


    public async Task<int> UpdateRecipeImage(int recipeId, string imagePath)
    {
        var recipe = _context.Recipes.FirstOrDefault(r => r.Id == recipeId);
        if (recipe == null)
        {
            return 0;
        }
        
        recipe.ImageUrl = imagePath;
        return await _context.SaveChangesAsync();
        
    }
}