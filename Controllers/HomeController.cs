using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingManagementSystem.Data;
using WeddingManagementSystem.Models;
using WeddingManagementSystem.ViewModels;

namespace WeddingManagementSystem.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ApplicationDbContext db, ILogger<HomeController> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var wedding = await _db.Weddings
            .Include(w => w.Tasks)
            .Include(w => w.Payments).ThenInclude(p => p.Vendor)
            .Include(w => w.Documents)
            .Include(w => w.StyleDecisions)
            .Include(w => w.Guests)
            .FirstOrDefaultAsync();

        if (wedding is null)
            return View("Empty");

        var today = DateTime.Today;
        var weekEnd = today.AddDays(7);

        var activeTasks = wedding.Tasks
            .Where(t => t.Status is not WeddingTaskStatus.Cancelled)
            .ToList();

        var completed = activeTasks.Count(t => t.Status == WeddingTaskStatus.Completed);
        var paid = wedding.Payments.Where(p => p.Status == PaymentStatus.Paid).Sum(p => p.Amount);

        var overdueTasks = activeTasks.Where(t => t.IsOverdue).OrderBy(t => t.DueDate).ToList();
        var nextStep = overdueTasks.FirstOrDefault()
            ?? activeTasks
                .Where(t => t.Status != WeddingTaskStatus.Completed)
                .OrderBy(t => t.DueDate ?? DateTime.MaxValue)
                .ThenBy(t => t.Priority)
                .FirstOrDefault();

        var upcomingPayments = wedding.Payments
            .Where(p => p.Status is PaymentStatus.Upcoming or PaymentStatus.Due or PaymentStatus.Overdue)
            .OrderBy(p => p.DueDate)
            .Take(5)
            .ToList();

        var guestEssentialsDone = 0;
        if (wedding.Guests.Any()) guestEssentialsDone++;
        if (wedding.Guests.Any(g => g.InvitationSent)) guestEssentialsDone++;
        if (wedding.Guests.Any(g => g.RsvpStatus == RsvpStatus.Accepted)) guestEssentialsDone++;
        if (wedding.Guests.Count(g => g.RsvpStatus != RsvpStatus.Pending) >= 3) guestEssentialsDone++;
        if (wedding.StyleDecisions.Any(s => s.IsApproved)) guestEssentialsDone++;
        if (wedding.Documents.Any()) guestEssentialsDone++;
        if (wedding.Payments.Any(p => p.Status == PaymentStatus.Paid)) guestEssentialsDone++;
        // seating / meal planning placeholder counted as done when style attire approved
        if (wedding.StyleDecisions.Any(s => s.Category == "Attire" && s.IsApproved)) guestEssentialsDone++;

        var vm = new DashboardViewModel
        {
            Wedding = wedding,
            DaysLeft = wedding.DaysLeft,
            TasksCompleted = completed,
            TasksTotal = activeTasks.Count,
            AmountPaid = paid,
            TotalBudget = wedding.TotalBudget,
            OverdueTasks = overdueTasks.Count,
            PaymentsOnTrack = wedding.Payments.Count(p => p.Status is PaymentStatus.Paid or PaymentStatus.Upcoming),
            TasksThisWeek = activeTasks.Count(t =>
                t.Status != WeddingTaskStatus.Completed &&
                t.DueDate.HasValue &&
                t.DueDate.Value.Date >= today &&
                t.DueDate.Value.Date <= weekEnd),
            PaymentsThisWeek = wedding.Payments.Count(p =>
                p.Status != PaymentStatus.Paid &&
                p.DueDate.HasValue &&
                p.DueDate.Value.Date >= today &&
                p.DueDate.Value.Date <= weekEnd),
            NextStepTask = nextStep,
            UploadContractTask = activeTasks.FirstOrDefault(t =>
                t.Title.Contains("Upload", StringComparison.OrdinalIgnoreCase) &&
                t.Status != WeddingTaskStatus.Completed),
            NextPayment = upcomingPayments.FirstOrDefault(),
            UpcomingDeadlines = activeTasks
                .Where(t => t.Status != WeddingTaskStatus.Completed && t.DueDate.HasValue)
                .OrderBy(t => t.DueDate)
                .Take(5)
                .ToList(),
            UpcomingPayments = upcomingPayments,
            RecentDocuments = wedding.Documents.OrderByDescending(d => d.UploadedAt).Take(5).ToList(),
            ApprovedStyles = wedding.StyleDecisions.Where(s => s.IsApproved).ToList(),
            GuestEssentialsDone = Math.Min(guestEssentialsDone, 8),
            GuestEssentialsTotal = 8
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkTaskDone(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task is not null)
        {
            task.Status = WeddingTaskStatus.Completed;
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
