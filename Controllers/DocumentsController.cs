using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingManagementSystem.Data;
using WeddingManagementSystem.Models;

namespace WeddingManagementSystem.Controllers;

public class DocumentsController : Controller
{
    private readonly ApplicationDbContext _db;

    public DocumentsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var docs = await _db.Documents.OrderByDescending(d => d.UploadedAt).ToListAsync();
        return View(docs);
    }

    public IActionResult Create() => View(new Document());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Document model)
    {
        var weddingId = await _db.GetCurrentWeddingIdAsync();
        if (weddingId is null) return RedirectToAction(nameof(Index));
        ModelState.Remove(nameof(Document.Wedding));
        ModelState.Remove(nameof(Document.WeddingId));
        if (!ModelState.IsValid) return View(model);
        model.WeddingId = weddingId.Value;
        model.UploadedAt = DateTime.UtcNow;
        _db.Documents.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var doc = await _db.Documents.FindAsync(id);
        if (doc is not null)
        {
            _db.Documents.Remove(doc);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
