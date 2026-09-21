using System.ComponentModel.DataAnnotations;

namespace WeddingManagementSystem.Models;

public class WebsiteRsvp
{
    public int Id { get; set; }

    public int WeddingId { get; set; }
    public Wedding Wedding { get; set; } = null!;

    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? Email { get; set; }

    public RsvpStatus Reply { get; set; } = RsvpStatus.Accepted;

    [MaxLength(1000)]
    public string? Message { get; set; }

    [MaxLength(300)]
    public string? DietaryNotes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
