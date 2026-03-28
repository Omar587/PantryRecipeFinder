
using RecipeFinder.Models;
using RecipeFinder.Models.FourmModel;


public class ForumComment
{
    public int Id { get; set; }
    public string Body { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;

    // Author
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }

    // Parent post
    public int ForumPostId { get; set; }
    public ForumPost ForumPost { get; set; }

    // Nested replies — null means this is a top-level comment
    public int? ParentCommentId { get; set; }
    public ForumComment? ParentComment { get; set; }
    public List<ForumComment> Replies { get; set; } = new();

    // Votes
    public List<ForumCommentVote> Votes { get; set; } = new();

    // Computed helpers (not mapped)
    public int VoteScore => Votes.Sum(v => v.Value);
}