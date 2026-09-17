using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingManagementSystem.Data;
using WeddingManagementSystem.Models;

namespace WeddingManagementSystem.Controllers;

public class TimelineController : Controller
{
    private readonly ApplicationDbContext _db;
    public TimelineController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var items = await _db.TimelineEvents.OrderBy(e => e.EventDate).ToListAsync();
        return View(items);
    }

    public IActionResult Create() => View(new TimelineEvent { EventDate = DateTime.Today });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TimelineEvent model)
    {
        var weddingId = await _db.GetCurrentWeddingIdAsync();
        if (weddingId is null) return RedirectToAction(nameof(Index));
        ModelState.Remove(nameof(TimelineEvent.Wedding));
        ModelState.Remove(nameof(TimelineEvent.WeddingId));
        if (!ModelState.IsValid) return View(model);
        model.WeddingId = weddingId.Value;
        _db.TimelineEvents.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _db.TimelineEvents.FindAsync(id);
        if (item is not null)
        {
            _db.TimelineEvents.Remove(item);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
