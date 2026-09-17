using System.ComponentModel.DataAnnotations;

namespace WeddingManagementSystem.Models;

public class InboxMessage
{
    public int Id { get; set; }
    public int WeddingId { get; set; }
    public Wedding Wedding { get; set; } = null!;

    [Required, MaxLength(150)]
    public string FromName { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? FromEmail { get; set; }

    [Required, MaxLength(200)]
    public string Subject { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string? Body { get; set; }

    public bool IsRead { get; set; }
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
}
