using System.ComponentModel.DataAnnotations;

namespace WeddingManagementSystem.Models;

public class ScheduleItem
{
    public int Id { get; set; }
    public int WeddingId { get; set; }
    public Wedding Wedding { get; set; } = null!;

    public TimeSpan StartTime { get; set; }

    public TimeSpan? EndTime { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Location { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public int SortOrder { get; set; }
}
