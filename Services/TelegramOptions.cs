namespace WeddingManagementSystem.Services;

public class TelegramOptions
{
    public const string SectionName = "Telegram";

    /// <summary>Bot token from @BotFather (e.g. 123456:ABC-DEF...).</summary>
    public string BotToken { get; set; } = "";

    /// <summary>Your chat id, group id, or channel id that receives alerts.</summary>
    public string ChatId { get; set; } = "";

    /// <summary>When false, RSVP still saves but no Telegram message is sent.</summary>
    public bool Enabled { get; set; }
}
