using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingManagementSystem.Data;
using WeddingManagementSystem.Models;

namespace WeddingManagementSystem.Controllers;

public class InboxController : Controller
{
    private readonly ApplicationDbContext _db;
    public InboxController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var items = await _db.InboxMessages.OrderByDescending(m => m.ReceivedAt).ToListAsync();
        return View(items);
    }

    public async Task<IActionResult> Details(int id)
    {
        var msg = await _db.InboxMessages.FindAsync(id);
        if (msg is null) return NotFound();
        if (!msg.IsRead)
        {
            msg.IsRead = true;
            await _db.SaveChangesAsync();
        }
        return View(msg);
    }

    public IActionResult Compose() => View(new InboxMessage());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Compose(InboxMessage model)
    {
        var weddingId = await _db.GetCurrentWeddingIdAsync();
        if (weddingId is null) return RedirectToAction(nameof(Index));
        ModelState.Remove(nameof(InboxMessage.Wedding));
        ModelState.Remove(nameof(InboxMessage.WeddingId));
        if (!ModelState.IsValid) return View(model);
        model.WeddingId = weddingId.Value;
        model.ReceivedAt = DateTime.UtcNow;
        model.IsRead = true;
        _db.InboxMessages.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
