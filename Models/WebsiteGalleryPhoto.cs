using System.ComponentModel.DataAnnotations;

namespace WeddingManagementSystem.Models;

public class WebsiteGalleryPhoto
{
    public int Id { get; set; }
    public int WebsiteSettingsId { get; set; }
    public WebsiteSettings WebsiteSettings { get; set; } = null!;

    [Required, MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Label { get; set; } = "Photo";

    public int SortOrder { get; set; }
}
