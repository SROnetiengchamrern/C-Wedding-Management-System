using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingManagementSystem.Data;
using WeddingManagementSystem.Models;

namespace WeddingManagementSystem.Controllers;

public class GuestsController : Controller
{
    private readonly ApplicationDbContext _db;
    public GuestsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var rows = await _db.Weddings
            .OrderBy(w => w.WebId)
            .Select(w => new GuestPartnerListItem
            {
                WeddingId = w.Id,
                WebId = w.WebId,
                CoupleName = w.Partner1Name + " & " + w.Partner2Name,
                WeddingDate = w.WeddingDate,
                GuestCount = w.Guests.Count,
                Accepted = w.Guests.Count(g => g.RsvpStatus == RsvpStatus.Accepted),
                Pending = w.Guests.Count(g => g.RsvpStatus == RsvpStatus.Pending),
                Declined = w.Guests.Count(g => g.RsvpStatus == RsvpStatus.Declined)
            })
            .ToListAsync();

        return View(rows);
    }

    public async Task<IActionResult> Manage(int weddingId)
    {
        var wedding = await _db.Weddings.AsNoTracking().FirstOrDefaultAsync(w => w.Id == weddingId);
        if (wedding is null) return NotFound();

        var guests = await _db.Guests
            .Where(g => g.WeddingId == weddingId)
            .OrderBy(g => g.FullName)
            .ToListAsync();

        ViewBag.WeddingId = wedding.Id;
        ViewBag.WebId = wedding.WebId;
        ViewBag.CoupleName = wedding.CoupleDisplayName;
        return View(guests);
    }

    public async Task<IActionResult> Create(int weddingId)
    {
        if (!await _db.Weddings.AnyAsync(w => w.Id == weddingId))
            return NotFound();

        ViewBag.WeddingId = weddingId;
        var wedding = await _db.Weddings.AsNoTracking().FirstAsync(w => w.Id == weddingId);
        ViewBag.CoupleName = wedding.CoupleDisplayName;
        return View(new Guest { WeddingId = weddingId, PartySize = 1 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Guest model)
    {
        if (!await _db.Weddings.AnyAsync(w => w.Id == model.WeddingId))
            return RedirectToAction(nameof(Index));

        ModelState.Remove(nameof(Guest.Wedding));

        if (!ModelState.IsValid)
        {
            ViewBag.WeddingId = model.WeddingId;
            var wedding = await _db.Weddings.AsNoTracking().FirstAsync(w => w.Id == model.WeddingId);
            ViewBag.CoupleName = wedding.CoupleDisplayName;
            return View(model);
        }

        _db.Guests.Add(model);
        await _db.SaveChangesAsync();
        TempData["Ok"] = "Guest added.";
        return RedirectToAction(nameof(Manage), new { weddingId = model.WeddingId });
    }
}

public class GuestPartnerListItem
{
    public int WeddingId { get; set; }
    public int WebId { get; set; }
    public string CoupleName { get; set; } = "";
    public DateTime WeddingDate { get; set; }
    public int GuestCount { get; set; }
    public int Accepted { get; set; }
    public int Pending { get; set; }
    public int Declined { get; set; }
}
