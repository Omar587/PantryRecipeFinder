namespace RecipeFinder.Models.FourmModel;



public class ForumPost
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Body { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public bool IsApproved { get; set; } = false;

    // Author
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }

    // Optional recipe link
    public int? RecipeId { get; set; }
    public Recipe? Recipe { get; set; }

    // Flair/tag
    public int? ForumFlairId { get; set; }
    public ForumFlair? Flair { get; set; }

    // Navigation properties
    public List<ForumComment> Comments { get; set; } = new();
    public List<ForumPostVote> Votes { get; set; } = new();

    // Computed helpers (not mapped)
    public int VoteScore => Votes.Sum(v => v.Value); // +1 or -1
}