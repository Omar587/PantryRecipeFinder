using Microsoft.EntityFrameworkCore;
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

    public List<Ingredient>  GetRecipeIngridients(int recipeId)
    {
        var ingridients = _context.Ingredients
            .Where(r => recipeId == r.RecipeId).ToList();

        return  ingridients;
    }


    public async Task<List<Recipe>> GetUsersFavoriteRecipes(int userId)
    {
        var favorites = await _context.FavoriteRecipes
            .Where(f => f.CustomerId == userId)
            .Include(f => f.Recipe)
            .OrderByDescending(f => f.AddedAt)
            .Select(f => f.Recipe)
            .ToListAsync();

        return favorites;

    }



}