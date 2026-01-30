namespace RecipeFinder.Models;

public class RecipeNote
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int RecipeId { get; set; }
    public string NoteText { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public Customer Customer { get; set; }
    public Recipe Recipe { get; set; }
}