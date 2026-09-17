using System.ComponentModel.DataAnnotations;

namespace WeddingManagementSystem.Models;

public class TimelineEvent
{
    public int Id { get; set; }
    public int WeddingId { get; set; }
    public Wedding Wedding { get; set; } = null!;

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public DateTime EventDate { get; set; }

    [MaxLength(100)]
    public string? Category { get; set; }
}
