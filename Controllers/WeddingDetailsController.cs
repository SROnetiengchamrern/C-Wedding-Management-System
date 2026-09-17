using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingManagementSystem.Data;
using WeddingManagementSystem.Models;

namespace WeddingManagementSystem.Controllers;

public class WeddingDetailsController : Controller
{
    private readonly ApplicationDbContext _db;
    public WeddingDetailsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var wedding = await _db.Weddings.Include(w => w.StyleDecisions).FirstOrDefaultAsync();
        return wedding is null ? RedirectToAction("Index", "Home") : View(wedding);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(Wedding model)
    {
        var wedding = await _db.Weddings.FindAsync(model.Id);
        if (wedding is null) return NotFound();

        wedding.Partner1Name = model.Partner1Name;
        wedding.Partner2Name = model.Partner2Name;
        wedding.WeddingDate = model.WeddingDate;
        wedding.VenueName = model.VenueName;
        wedding.VenueLocation = model.VenueLocation;
        wedding.TotalBudget = model.TotalBudget;
        wedding.CeremonyNotes = model.CeremonyNotes;
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Wedding details saved.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddStyle(string category, string decision)
    {
        var weddingId = await _db.GetCurrentWeddingIdAsync();
        if (weddingId is null || string.IsNullOrWhiteSpace(category)) return RedirectToAction(nameof(Index));

        _db.StyleDecisions.Add(new StyleDecision
        {
            WeddingId = weddingId.Value,
            Category = category.Trim(),
            Decision = decision?.Trim(),
            IsApproved = true
        });
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
