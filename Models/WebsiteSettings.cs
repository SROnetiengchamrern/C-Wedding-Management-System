using System.ComponentModel.DataAnnotations;

namespace WeddingManagementSystem.Models;

public class WebsiteSettings
{
    public int Id { get; set; }
    public int WeddingId { get; set; }
    public Wedding Wedding { get; set; } = null!;

    [Required, MaxLength(150)]
    public string SiteTitle { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Slug { get; set; }

    [MaxLength(1000)]
    public string? WelcomeMessage { get; set; }

    [MaxLength(500)]
    public string? RsvpUrl { get; set; }

    public bool IsPublished { get; set; }
}
