using RecipeFinder.Models.FourmModel;

namespace RecipeFinder.Models;

// Upvote/downvote on a post
public class ForumPostVote
{
    public int Id { get; set; }

    // +1 for upvote, -1 for downvote
    public int Value { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; }

    public int ForumPostId { get; set; }
    public ForumPost ForumPost { get; set; }
}

// Upvote/downvote on a comment
public class ForumCommentVote
{
    public int Id { get; set; }

    // +1 for upvote, -1 for downvote
    public int Value { get; set; }

    public int CustomerId { get; set; }
    public Customer Customer { get; set; }

    public int ForumCommentId { get; set; }
    public ForumComment ForumComment { get; set; }
}

// Flair/tags that can be assigned to a post (e.g. "Question", "Recipe Share", "Tip", "Discussion")
public class ForumFlair
{
    public int Id { get; set; }
    public string Name { get; set; }       // e.g. "Recipe Share"
    public string ColorHex { get; set; }   // e.g. "#E63946" for display in UI

    public List<ForumPost> Posts { get; set; } = new();
}