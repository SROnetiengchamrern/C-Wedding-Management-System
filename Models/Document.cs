using System.ComponentModel.DataAnnotations;

namespace WeddingManagementSystem.Models;

public class Document
{
    public int Id { get; set; }

    public int WeddingId { get; set; }
    public Wedding Wedding { get; set; } = null!;

    [Required, MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Category { get; set; }

    [MaxLength(500)]
    public string? FilePath { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
