using RecipeFinder.Models.Enums;

namespace RecipeFinder.Models;


public class Recipe
{
    public int Id { get; set; }

    public string Name { get; set; }
    public string ImageUrl { get; set; }

    public Cuisine Cuisine { get; set; }
    public Difficulty Difficulty { get; set; }

    public int PrepTime { get; set; }
    public int CookTime { get; set; }
    public int Servings { get; set; }

    public double Rating { get; set; }
    public int RatingCount { get; set; }

    public DietaryInfo Dietary { get; set; }

    public List<Ingredient> Ingredients { get; set; } = new();
    public List<Tag> Tags { get; set; } = new();

    // Domain logic belongs here
    public int TotalTime() => PrepTime + CookTime;
}
