using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingManagementSystem.Data;
using WeddingManagementSystem.ViewModels;

namespace WeddingManagementSystem.Controllers;

public class TotalsController : Controller
{
    private const int DefaultWebId = 2027;
    private readonly ApplicationDbContext _db;

    public TotalsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index(int? webId)
    {
        var partners = await _db.Weddings
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

        if (partners.Count == 0)
            return View(new GiftTotalsPageViewModel());

        var selectedWebId = webId ?? DefaultWebId;
        var selected = partners.FirstOrDefault(p => p.WebId == selectedWebId)
                       ?? partners.FirstOrDefault(p => p.WebId == DefaultWebId)
                       ?? partners[0];

        return View(new GiftTotalsPageViewModel
        {
            Partners = partners,
            SelectedWebId = selected.WebId,
            Selected = selected
        });
    }
}
