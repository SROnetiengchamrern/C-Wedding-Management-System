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
        var guests = await _db.Guests.OrderBy(g => g.FullName).ToListAsync();
        return View(guests);
    }

    public IActionResult Create() => View(new Guest());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Guest model)
    {
        var wedding = await _db.Weddings.FirstOrDefaultAsync();
        if (wedding is null) return RedirectToAction(nameof(Index));

        ModelState.Remove(nameof(Guest.Wedding));
        ModelState.Remove(nameof(Guest.WeddingId));

        if (!ModelState.IsValid)
            return View(model);

        model.WeddingId = wedding.Id;
        _db.Guests.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
