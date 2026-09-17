using System.ComponentModel.DataAnnotations;

namespace WeddingManagementSystem.Models;

public class StyleDecision
{
    public int Id { get; set; }

    public int WeddingId { get; set; }
    public Wedding Wedding { get; set; } = null!;

    [Required, MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [MaxLength(300)]
    public string? Decision { get; set; }

    public bool IsApproved { get; set; }
}
