namespace RecipeFinder.Models;

public class DietaryInfo
{
    public int Id { get; set; }

    public bool Vegetarian { get; set; }
    public bool Vegan { get; set; }
    public bool GlutenFree { get; set; }
    public bool DairyFree { get; set; }

    public int RecipeId { get; set; }
}
