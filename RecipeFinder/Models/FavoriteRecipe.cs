namespace RecipeFinder.Models;

public class FavoriteRecipe
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int RecipeId { get; set; }
    public DateTime AddedAt { get; set; }
    
    // Navigation properties
    public Customer Customer { get; set; }
    public Recipe Recipe { get; set; }
}