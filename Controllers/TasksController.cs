using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingManagementSystem.Data;
using WeddingManagementSystem.Models;

namespace WeddingManagementSystem.Controllers;

public class TasksController : Controller
{
    private readonly ApplicationDbContext _db;

    public TasksController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var tasks = await _db.Tasks
            .OrderBy(t => t.Status == WeddingTaskStatus.Completed)
            .ThenBy(t => t.DueDate)
            .ToListAsync();
        return View(tasks);
    }

    public IActionResult Create() => View(new WeddingTask());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WeddingTask model)
    {
        var wedding = await _db.Weddings.FirstOrDefaultAsync();
        if (wedding is null) return RedirectToAction(nameof(Index));

        ModelState.Remove(nameof(WeddingTask.Wedding));
        ModelState.Remove(nameof(WeddingTask.WeddingId));

        if (!ModelState.IsValid)
            return View(model);

        model.WeddingId = wedding.Id;
        model.CreatedAt = DateTime.UtcNow;
        _db.Tasks.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        return task is null ? NotFound() : View(task);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, WeddingTask model)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task is null) return NotFound();

        if (!ModelState.IsValid)
            return View(model);

        task.Title = model.Title;
        task.Description = model.Description;
        task.DueDate = model.DueDate;
        task.Status = model.Status;
        task.Priority = model.Priority;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task is not null)
        {
            task.Status = WeddingTaskStatus.Completed;
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
