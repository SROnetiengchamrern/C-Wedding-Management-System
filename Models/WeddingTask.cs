using System.ComponentModel.DataAnnotations;

namespace WeddingManagementSystem.Models;

public enum WeddingTaskStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3
}

public class WeddingTask
{
    public int Id { get; set; }

    public int WeddingId { get; set; }
    public Wedding Wedding { get; set; } = null!;

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public WeddingTaskStatus Status { get; set; } = WeddingTaskStatus.Pending;

    public int Priority { get; set; } = 2;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsOverdue =>
        Status != WeddingTaskStatus.Completed &&
        Status != WeddingTaskStatus.Cancelled &&
        DueDate.HasValue &&
        DueDate.Value.Date < DateTime.Today;
}
