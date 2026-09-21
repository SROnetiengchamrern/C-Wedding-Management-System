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

    [MaxLength(1000)]
    public string? ShareImageUrl { get; set; }

    [MaxLength(300)]
    public string? ShareDescription { get; set; }

    /// <summary>Invite layout theme 1–7.</summary>
    public int ThemeLayout { get; set; } = 6;

    public bool ShowMap { get; set; } = true;

    [MaxLength(300)]
    public string? MapSearch { get; set; }

    [MaxLength(2000)]
    public string? InviteMessage { get; set; }

    [MaxLength(1000)]
    public string? CoverImageUrl { get; set; }

    [MaxLength(500)]
    public string? MusicUrl { get; set; }

    public bool IsPublished { get; set; }

    public ICollection<WebsiteBankQr> BankQrs { get; set; } = new List<WebsiteBankQr>();
    public ICollection<WebsiteGalleryPhoto> GalleryPhotos { get; set; } = new List<WebsiteGalleryPhoto>();
}
