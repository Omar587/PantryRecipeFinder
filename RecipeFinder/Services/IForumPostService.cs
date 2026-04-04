using RecipeFinder.ViewModels.Forum;
using RecipeFinder.Models.FourmModel;

namespace RecipeFinder.Services.Forum;

public interface IForumPostService
{
    /// <summary>Returns a paginated list of posts. Sort: "hot" | "new" | "top".</summary>
    Task<ForumIndexViewModel> GetIndexViewModelAsync(
        string sort, int page, int pageSize, int? flairId, int? currentCustomerId);

    /// <summary>Returns the full post + threaded comments, or null if not found.</summary>
    Task<ForumPostDetailViewModel?> GetDetailViewModelAsync(int postId, int? currentCustomerId);

    /// <summary>Returns a blank create-form view model with dropdown data pre-populated.</summary>
    Task<CreatePostViewModel> GetCreateViewModelAsync();

    /// <summary>Persists a new post and returns its Id.</summary>
    Task<int> CreatePostAsync(CreatePostViewModel vm, int customerId);

    /// <summary>Updates title+body if the customer owns the post. Returns false if not found or unauthorized.</summary>
    Task<bool> EditPostAsync(EditPostViewModel vm, int customerId);

    /// <summary>Soft-deletes a post. Returns false if not found or unauthorized.</summary>
    Task<bool> DeletePostAsync(int postId, int customerId);

    /// <summary>
    /// Toggles or sets a vote. Passing the same value twice removes the vote (Reddit-style).
    /// Returns the updated vote result.
    /// </summary>
    Task<VoteResultViewModel> VoteAsync(int postId, int customerId, int value);
}
