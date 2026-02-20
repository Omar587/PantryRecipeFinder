using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeFinder.Data;
using RecipeFinder.Models;

namespace RecipeFinder.Controllers;

[Authorize]
public class NotesController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<Customer> _userManager;

    public NotesController(AppDbContext db, UserManager<Customer> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(_userManager.GetUserId(User)!);

        var notes = await _db.RecipeNotes
            .Where(n => n.CustomerId == userId)
            .Include(n => n.Recipe)
            .OrderByDescending(n => n.UpdatedAt ?? n.CreatedAt)
            .ToListAsync();

        return View(notes);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, string noteText)
    {
        var userId = int.Parse(_userManager.GetUserId(User)!);

        var note = await _db.RecipeNotes
            .FirstOrDefaultAsync(n => n.Id == id && n.CustomerId == userId);

        if (note == null) return NotFound();

        note.NoteText = noteText;
        note.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = int.Parse(_userManager.GetUserId(User)!);

        var note = await _db.RecipeNotes
            .FirstOrDefaultAsync(n => n.Id == id && n.CustomerId == userId);

        if (note != null)
        {
            _db.RecipeNotes.Remove(note);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}