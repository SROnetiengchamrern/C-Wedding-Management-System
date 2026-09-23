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

    public async Task<IActionResult> Index()
    {
        var rows = await _db.Weddings
            .OrderBy(w => w.WebId)
            .Select(w => new GiftPartnerListItem
            {
                WeddingId = w.Id,
                WebId = w.WebId,
                CoupleName = w.Partner1Name + " & " + w.Partner2Name,
                WeddingDate = w.WeddingDate,
                WeddingDateEnd = w.WeddingDateEnd,
                VenueName = w.VenueName,
                GiftCount = w.WeddingGifts.Count,
                TotalKhr = w.WeddingGifts.Sum(g => (decimal?)g.AmountKhr) ?? 0,
                TotalUsd = w.WeddingGifts.Sum(g => (decimal?)g.AmountUsd) ?? 0
            })
            .ToListAsync();

        return View(rows);
    }

    public async Task<IActionResult> Manage(int weddingId, string? search, int? editId)
    {
        var wedding = await _db.Weddings.AsNoTracking().FirstOrDefaultAsync(w => w.Id == weddingId);
        if (wedding is null) return NotFound();

        var gifts = await FilterGifts(weddingId, null);
        WeddingGift form;

        if (editId is > 0)
        {
            form = await _db.WeddingGifts.AsNoTracking()
                       .FirstOrDefaultAsync(g => g.Id == editId && g.WeddingId == weddingId)
                   ?? new WeddingGift { GiftDate = DateTime.Today, WeddingId = weddingId };
        }
        else
        {
            form = new WeddingGift { GiftDate = DateTime.Today, WeddingId = weddingId };
        }

        return View(new GiftMoneyViewModel
        {
            WeddingId = wedding.Id,
            WebId = wedding.WebId,
            CoupleName = wedding.CoupleDisplayName,
            Form = form,
            Gifts = gifts,
            Search = search
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int weddingId, [Bind(Prefix = "Form")] WeddingGift form, string? search)
    {
        if (!await _db.Weddings.AnyAsync(w => w.Id == weddingId))
            return RedirectToAction(nameof(Index));

        ClearGiftNavState();

        if (string.IsNullOrWhiteSpace(form.GuestName))
        {
            TempData["Error"] = "Guest name is required. You can type English or Khmer.";
            return RedirectToAction(nameof(Manage), new { weddingId, search });
        }

        Normalize(form);
        form.Id = 0;
        form.WeddingId = weddingId;
        form.Wedding = null!;

        _db.WeddingGifts.Add(form);
        await _db.SaveChangesAsync();

        TempData["Ok"] = "Guest gift saved.";
        return RedirectToAction(nameof(Manage), new { weddingId, search });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int weddingId, [Bind(Prefix = "Form")] WeddingGift form, string? search)
    {
        ClearGiftNavState();

        if (form.Id <= 0)
            return RedirectToAction(nameof(Manage), new { weddingId, search });

        if (string.IsNullOrWhiteSpace(form.GuestName))
        {
            TempData["Error"] = "Guest name is required. You can type English or Khmer.";
            return RedirectToAction(nameof(Manage), new { weddingId, search, editId = form.Id });
        }

        var existing = await _db.WeddingGifts.FirstOrDefaultAsync(g => g.Id == form.Id && g.WeddingId == weddingId);
        if (existing is null)
        {
            TempData["Error"] = "Guest not found.";
            return RedirectToAction(nameof(Manage), new { weddingId, search });
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
        return RedirectToAction(nameof(Manage), new { weddingId, search });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int weddingId, string? search)
    {
        var gift = await _db.WeddingGifts.FirstOrDefaultAsync(g => g.Id == id && g.WeddingId == weddingId);
        if (gift is not null)
        {
            _db.WeddingGifts.Remove(gift);
            await _db.SaveChangesAsync();
            TempData["Ok"] = "Guest removed.";
        }
        return RedirectToAction(nameof(Manage), new { weddingId, search });
    }

    public async Task<IActionResult> ExportExcel(int weddingId, string? search)
    {
        var wedding = await _db.Weddings.AsNoTracking().FirstOrDefaultAsync(w => w.Id == weddingId);
        if (wedding is null) return NotFound();

        var gifts = await FilterGifts(weddingId, search);
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
        var slug = $"{wedding.Partner1Name}-{wedding.Partner2Name}".ToLowerInvariant().Replace(' ', '-');
        return File(bytes, "text/csv", $"wedding-gifts-{slug}-{DateTime.Today:yyyyMMdd}.csv");
    }

    public async Task<IActionResult> ExportPdf(int weddingId, string? search)
    {
        var wedding = await _db.Weddings.AsNoTracking().FirstOrDefaultAsync(w => w.Id == weddingId);
        if (wedding is null) return NotFound();

        var gifts = await FilterGifts(weddingId, search);
        ViewBag.CoupleName = wedding.CoupleDisplayName;
        ViewBag.WebId = wedding.WebId;
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

    private async Task<List<WeddingGift>> FilterGifts(int weddingId, string? search)
    {
        var query = _db.WeddingGifts.Where(g => g.WeddingId == weddingId);
        if (!string.IsNullOrWhiteSpace(search))
        {
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
