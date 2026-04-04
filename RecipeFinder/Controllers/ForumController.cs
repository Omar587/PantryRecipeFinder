using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RecipeFinder.Models;
using RecipeFinder.Services.Forum;
using RecipeFinder.ViewModels.Forum;

namespace RecipeFinder.Controllers;

public class ForumController : Controller
{
    private readonly IForumPostService _posts;
    private readonly UserManager<Customer> _userManager;

    public ForumController(
        IForumPostService posts,
        UserManager<Customer> userManager)
    {
        _posts = posts;
        _userManager = userManager;
    }

    // ─── Resolve logged-in customer ID from Identity ──────────────────────────
    private int? CurrentCustomerId
    {
        get
        {
            var userId = _userManager.GetUserId(User);
            return int.TryParse(userId, out var id) ? id : null;
        }
    }

    // ─── GET /Forum ───────────────────────────────────────────────────────────

    public async Task<IActionResult> Index(string sort = "hot", int page = 1, int? flairId = null)
    {
        const int PageSize = 15;

        var vm = await _posts.GetIndexViewModelAsync(
            sort,
            page,
            PageSize,
            flairId,
            CurrentCustomerId
        );

        return View(vm);
    }

    // ─── GET /Forum/Details/5 ─────────────────────────────────────────────────

    public async Task<IActionResult> Details(int id)
    {
        var vm = await _posts.GetDetailViewModelAsync(id, CurrentCustomerId);

        if (vm is null)
            return NotFound();

        return View(vm);
    }

    // ─── GET /Forum/Create ────────────────────────────────────────────────────

    [Authorize]
    public async Task<IActionResult> Create()
    {
        var vm = await _posts.GetCreateViewModelAsync();
        return View(vm);
    }

    // ─── POST /Forum/Create ───────────────────────────────────────────────────

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePostViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            var fresh = await _posts.GetCreateViewModelAsync();
            vm.AvailableFlairs = fresh.AvailableFlairs;
            vm.AvailableRecipes = fresh.AvailableRecipes;
            return View(vm);
        }

        if (CurrentCustomerId == null)
            return Unauthorized();

        int newId = await _posts.CreatePostAsync(vm, CurrentCustomerId.Value);

        return RedirectToAction(nameof(Details), new { id = newId });
    }

    // ─── GET /Forum/Edit/5 ────────────────────────────────────────────────────

    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var detail = await _posts.GetDetailViewModelAsync(id, CurrentCustomerId);

        if (detail is null)
            return NotFound();

        if (!detail.CanEdit)
            return Forbid();

        return View(new EditPostViewModel
        {
            Id = detail.Id,
            Title = detail.Title,
            Body = detail.Body
        });
    }

    // ─── POST /Forum/Edit/5 ───────────────────────────────────────────────────

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditPostViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        if (CurrentCustomerId == null)
            return Unauthorized();

        bool ok = await _posts.EditPostAsync(vm, CurrentCustomerId.Value);

        if (!ok)
            return Forbid();

        return RedirectToAction(nameof(Details), new { id = vm.Id });
    }

    // ─── POST /Forum/Delete/5 ─────────────────────────────────────────────────

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (CurrentCustomerId == null)
            return Unauthorized();

        bool ok = await _posts.DeletePostAsync(id, CurrentCustomerId.Value);

        if (!ok)
            return Forbid();

        return RedirectToAction(nameof(Index));
    }

    // ─── POST /Forum/Vote (AJAX) ──────────────────────────────────────────────

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Vote(int postId, int value)
    {
        if (CurrentCustomerId == null)
            return Unauthorized();

        var result = await _posts.VoteAsync(postId, CurrentCustomerId.Value, value);

        return Json(result);
    }
}