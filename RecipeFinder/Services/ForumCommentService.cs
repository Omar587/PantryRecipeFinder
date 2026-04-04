using Microsoft.EntityFrameworkCore;
using RecipeFinder.Data;
using RecipeFinder.Models;
using RecipeFinder.Models.FourmModel;
using RecipeFinder.ViewModels.Forum;

namespace RecipeFinder.Services.Forum;

// ─── INTERFACE ────────────────────────────────────────────────────────────────

public interface IForumCommentService
{
    Task<bool> AddCommentAsync(AddCommentViewModel vm, int customerId);
    Task<bool> EditCommentAsync(int commentId, string body, int customerId);
    Task<bool> DeleteCommentAsync(int commentId, int customerId);
    Task<VoteResultViewModel> VoteAsync(int commentId, int customerId, int value);
}

// ─── IMPLEMENTATION ───────────────────────────────────────────────────────────

public class ForumCommentService : IForumCommentService
{
    private readonly AppDbContext _db;   // ← replace with your actual DbContext type

    public ForumCommentService(AppDbContext db) => _db = db;

    public async Task<bool> AddCommentAsync(AddCommentViewModel vm, int customerId)
    {
        // Validate the parent post exists
        bool postExists = await _db.ForumPosts
            .AnyAsync(p => p.Id == vm.ForumPostId && !p.IsDeleted);

        if (!postExists) return false;

        // If it's a reply, validate the parent comment is on the same post
        if (vm.ParentCommentId.HasValue)
        {
            bool parentValid = await _db.ForumComments
                .AnyAsync(c => c.Id == vm.ParentCommentId
                            && c.ForumPostId == vm.ForumPostId
                            && !c.IsDeleted);
            if (!parentValid) return false;
        }

        _db.ForumComments.Add(new ForumComment
        {
            Body            = vm.Body.Trim(),
            CustomerId      = customerId,
            ForumPostId     = vm.ForumPostId,
            ParentCommentId = vm.ParentCommentId,
            CreatedAt       = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EditCommentAsync(int commentId, string body, int customerId)
    {
        var comment = await _db.ForumComments
            .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);

        if (comment is null || comment.CustomerId != customerId) return false;

        comment.Body      = body.Trim();
        comment.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteCommentAsync(int commentId, int customerId)
    {
        var comment = await _db.ForumComments
            .FirstOrDefaultAsync(c => c.Id == commentId && !c.IsDeleted);

        if (comment is null || comment.CustomerId != customerId) return false;

        comment.IsDeleted = true;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<VoteResultViewModel> VoteAsync(int commentId, int customerId, int value)
    {
        value = value >= 0 ? 1 : -1;

        var existing = await _db.ForumCommentVotes
            .FirstOrDefaultAsync(v => v.ForumCommentId == commentId && v.CustomerId == customerId);

        if (existing is not null)
        {
            if (existing.Value == value)
            {
                _db.ForumCommentVotes.Remove(existing);
                value = 0;
            }
            else
            {
                existing.Value = value;
            }
        }
        else
        {
            _db.ForumCommentVotes.Add(new ForumCommentVote
            {
                ForumCommentId = commentId,
                CustomerId     = customerId,
                Value          = value
            });
        }

        await _db.SaveChangesAsync();

        int newScore = await _db.ForumCommentVotes
            .Where(v => v.ForumCommentId == commentId)
            .SumAsync(v => v.Value);

        return new VoteResultViewModel { Success = true, NewScore = newScore, UserVote = value };
    }
}
