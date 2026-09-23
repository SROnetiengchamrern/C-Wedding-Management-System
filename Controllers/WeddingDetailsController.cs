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
        var weddings = await _db.Weddings
            .Include(w => w.WebsiteSettings)
            .OrderBy(w => w.WebId)
            .ToListAsync();
        return View(weddings);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var wedding = await _db.Weddings.FindAsync(id);
        return wedding is null ? NotFound() : View(wedding);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(Wedding model)
    {
        var wedding = await _db.Weddings.FindAsync(model.Id);
        if (wedding is null) return NotFound();

        wedding.Partner1Name = model.Partner1Name?.Trim() ?? wedding.Partner1Name;
        wedding.Partner2Name = model.Partner2Name?.Trim() ?? wedding.Partner2Name;
        wedding.WeddingDate = model.WeddingDate.Date;
        wedding.WeddingDateEnd = model.WeddingDateEnd?.Date;
        if (wedding.WeddingDateEnd is DateTime end && end < wedding.WeddingDate)
            wedding.WeddingDateEnd = wedding.WeddingDate;
        if (wedding.WeddingDateEnd == wedding.WeddingDate)
            wedding.WeddingDateEnd = null;
        wedding.VenueName = string.IsNullOrWhiteSpace(model.VenueName) ? null : model.VenueName.Trim();
        wedding.VenueLocation = string.IsNullOrWhiteSpace(model.VenueLocation) ? null : model.VenueLocation.Trim();
        wedding.CeremonyNotes = string.IsNullOrWhiteSpace(model.CeremonyNotes) ? null : model.CeremonyNotes.Trim();
        await _db.SaveChangesAsync();

        if (wedding.WebsiteSettings is null)
            await _db.Entry(wedding).Reference(w => w.WebsiteSettings).LoadAsync();
        if (wedding.WebsiteSettings is not null)
        {
            wedding.WebsiteSettings.SiteTitle = wedding.CoupleDisplayName;
            await _db.SaveChangesAsync();
        }

        TempData["Ok"] = "Couple details saved.";
        return RedirectToAction(nameof(Index));
    }
}
