using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingManagementSystem.Data;
using WeddingManagementSystem.Models;

namespace WeddingManagementSystem.Controllers;

public class AgreementsController : Controller
{
    private readonly ApplicationDbContext _db;
    public AgreementsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var items = await _db.Agreements.OrderByDescending(a => a.Id).ToListAsync();
        return View(items);
    }

    public IActionResult Create() => View(new Agreement());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Agreement model)
    {
        var weddingId = await _db.GetCurrentWeddingIdAsync();
        if (weddingId is null) return RedirectToAction(nameof(Index));
        ModelState.Remove(nameof(Agreement.Wedding));
        ModelState.Remove(nameof(Agreement.WeddingId));
        if (!ModelState.IsValid) return View(model);
        model.WeddingId = weddingId.Value;
        if (model.Status == AgreementStatus.Signed && model.SignedAt is null)
            model.SignedAt = DateTime.UtcNow;
        _db.Agreements.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkSigned(int id)
    {
        var item = await _db.Agreements.FindAsync(id);
        if (item is not null)
        {
            item.Status = AgreementStatus.Signed;
            item.SignedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
