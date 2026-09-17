using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WeddingManagementSystem.Data;
using WeddingManagementSystem.Models;

namespace WeddingManagementSystem.Controllers;

public class BudgetController : Controller
{
    private readonly ApplicationDbContext _db;

    public BudgetController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var wedding = await _db.Weddings.FirstOrDefaultAsync();
        var payments = await _db.Payments
            .Include(p => p.Vendor)
            .OrderBy(p => p.DueDate)
            .ToListAsync();

        ViewBag.Wedding = wedding;
        ViewBag.TotalBudget = wedding?.TotalBudget ?? 0;
        ViewBag.Paid = payments.Where(p => p.Status == PaymentStatus.Paid).Sum(p => p.Amount);
        return View(payments);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Vendors = new SelectList(await _db.Vendors.OrderBy(v => v.Name).ToListAsync(), "Id", "Name");
        return View(new Payment { DueDate = DateTime.Today.AddDays(14), Status = PaymentStatus.Upcoming });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Payment model)
    {
        var weddingId = await _db.GetCurrentWeddingIdAsync();
        if (weddingId is null) return RedirectToAction(nameof(Index));
        ModelState.Remove(nameof(Payment.Wedding));
        ModelState.Remove(nameof(Payment.WeddingId));
        ModelState.Remove(nameof(Payment.Vendor));
        if (!ModelState.IsValid)
        {
            ViewBag.Vendors = new SelectList(await _db.Vendors.OrderBy(v => v.Name).ToListAsync(), "Id", "Name", model.VendorId);
            return View(model);
        }
        model.WeddingId = weddingId.Value;
        _db.Payments.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkPaid(int id)
    {
        var payment = await _db.Payments.FindAsync(id);
        if (payment is not null)
        {
            payment.Status = PaymentStatus.Paid;
            payment.PaidDate = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
