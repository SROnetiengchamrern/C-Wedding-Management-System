using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingManagementSystem.Data;
using WeddingManagementSystem.Models;
using WeddingManagementSystem.ViewModels;

namespace WeddingManagementSystem.Controllers;

public class GiftsController : Controller
{
    private readonly ApplicationDbContext _db;

    public GiftsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? search, int? editId)
    {
        // Always load full list so DataTables can filter Khmer/English client-side.
        var gifts = await FilterGifts(null);
        WeddingGift form;

        if (editId is > 0)
        {
            form = await _db.WeddingGifts.AsNoTracking().FirstOrDefaultAsync(g => g.Id == editId)
                   ?? new WeddingGift { GiftDate = DateTime.Today };
        }
        else
        {
            form = new WeddingGift { GiftDate = DateTime.Today };
        }

        return View(new GiftMoneyViewModel
        {
            Form = form,
            Gifts = gifts,
            Search = search
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add([Bind(Prefix = "Form")] WeddingGift form, string? search)
    {
        var weddingId = await _db.GetCurrentWeddingIdAsync();
        if (weddingId is null)
            return RedirectToAction(nameof(Index));

        ClearGiftNavState();

        if (string.IsNullOrWhiteSpace(form.GuestName))
        {
            TempData["Error"] = "Guest name is required. You can type English or Khmer.";
            return RedirectToAction(nameof(Index), new { search });
        }

        Normalize(form);
        form.Id = 0;
        form.WeddingId = weddingId.Value;
        form.Wedding = null!;

        _db.WeddingGifts.Add(form);
        await _db.SaveChangesAsync();

        TempData["Ok"] = "Guest gift saved.";
        return RedirectToAction(nameof(Index), new { search });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update([Bind(Prefix = "Form")] WeddingGift form, string? search)
    {
        ClearGiftNavState();

        if (form.Id <= 0)
            return RedirectToAction(nameof(Index), new { search });

        if (string.IsNullOrWhiteSpace(form.GuestName))
        {
            TempData["Error"] = "Guest name is required. You can type English or Khmer.";
            return RedirectToAction(nameof(Index), new { search, editId = form.Id });
        }

        var existing = await _db.WeddingGifts.FindAsync(form.Id);
        if (existing is null)
        {
            TempData["Error"] = "Guest not found.";
            return RedirectToAction(nameof(Index), new { search });
        }

        Normalize(form);
        existing.GuestName = form.GuestName;
        existing.AmountKhr = form.AmountKhr;
        existing.AmountUsd = form.AmountUsd;
        existing.Address = form.Address;
        existing.Relationship = form.Relationship;
        existing.GiftDate = form.GiftDate;
        existing.Notes = form.Notes;

        await _db.SaveChangesAsync();
        TempData["Ok"] = "Guest gift updated.";
        return RedirectToAction(nameof(Index), new { search });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, string? search)
    {
        var gift = await _db.WeddingGifts.FindAsync(id);
        if (gift is not null)
        {
            _db.WeddingGifts.Remove(gift);
            await _db.SaveChangesAsync();
            TempData["Ok"] = "Guest removed.";
        }
        return RedirectToAction(nameof(Index), new { search });
    }

    public async Task<IActionResult> ExportExcel(string? search)
    {
        var gifts = await FilterGifts(search);
        var sb = new StringBuilder();
        sb.AppendLine("No,Guest,Amount KHR,Amount USD,Address,Relationship,Gift Date,Notes");
        var i = 1;
        foreach (var g in gifts)
        {
            sb.AppendLine(string.Join(",",
                i++,
                Csv(g.GuestName),
                g.AmountKhr,
                g.AmountUsd,
                Csv(g.Address),
                Csv(g.Relationship),
                g.GiftDate.ToString("yyyy-MM-dd"),
                Csv(g.Notes)));
        }

        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        return File(bytes, "text/csv", $"wedding-gifts-{DateTime.Today:yyyyMMdd}.csv");
    }

    public async Task<IActionResult> ExportPdf(string? search)
    {
        var gifts = await FilterGifts(search);
        return View(gifts);
    }

    private void ClearGiftNavState()
    {
        ModelState.Remove(nameof(WeddingGift.Wedding));
        ModelState.Remove(nameof(WeddingGift.WeddingId));
        ModelState.Remove("Form.Wedding");
        ModelState.Remove("Form.WeddingId");
    }

    private static void Normalize(WeddingGift form)
    {
        form.GuestName = form.GuestName.Trim();
        form.Address = form.Address?.Trim();
        form.Relationship = form.Relationship?.Trim();
        form.Notes = form.Notes?.Trim();
        if (form.GiftDate == default)
            form.GiftDate = DateTime.Today;
    }

    private async Task<List<WeddingGift>> FilterGifts(string? search)
    {
        var query = _db.WeddingGifts.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            // Unicode-safe LIKE for Khmer + English (nvarchar).
            var term = EscapeLike(search.Trim());
            var pattern = $"%{term}%";
            query = query.Where(g =>
                EF.Functions.Like(g.GuestName, pattern) ||
                (g.Address != null && EF.Functions.Like(g.Address, pattern)) ||
                (g.Notes != null && EF.Functions.Like(g.Notes, pattern)) ||
                (g.Relationship != null && EF.Functions.Like(g.Relationship, pattern)));
        }

        return await query
            .OrderByDescending(g => g.GiftDate)
            .ThenBy(g => g.GuestName)
            .ToListAsync();
    }

    private static string EscapeLike(string value) =>
        value.Replace("[", "[[]", StringComparison.Ordinal)
             .Replace("%", "[%]", StringComparison.Ordinal)
             .Replace("_", "[_]", StringComparison.Ordinal);

    private static string Csv(string? value)
    {
        value ??= "";
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}
