using System.ComponentModel.DataAnnotations;

namespace WeddingManagementSystem.Models;

public class WebsiteCoverPhoto
{
    public int Id { get; set; }
    public int WebsiteSettingsId { get; set; }
    public WebsiteSettings WebsiteSettings { get; set; } = null!;

    [Required, MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Label { get; set; } = "Cover";

    public int SortOrder { get; set; }
}
