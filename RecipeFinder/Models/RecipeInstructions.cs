namespace RecipeFinder.Models;

public class RecipeInstructions
{
    public int Id {get; set;}
    public int RecipeId {get; set;}
    public int StepNumber {get; set;}
    public string Instruction {get; set;}
    
    
    
}