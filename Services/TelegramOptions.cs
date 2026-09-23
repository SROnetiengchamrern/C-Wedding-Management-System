namespace WeddingManagementSystem.Services;

public class TelegramOptions
{
    public const string SectionName = "Telegram";

    /// <summary>Default bot token from @BotFather (used when a couple has no BotToken override).</summary>
    public string BotToken { get; set; } = "";

    /// <summary>Default chat id when a couple has no Couples entry.</summary>
    public string ChatId { get; set; } = "";

    /// <summary>When false, RSVP still saves but no Telegram message is sent.</summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Optional per-couple routing by Web ID.
    /// Example key: "2027" → { "ChatId": "...", "BotToken": "..." }.
    /// Empty BotToken/ChatId falls back to the default Telegram values above.
    /// </summary>
    public Dictionary<string, TelegramCoupleOptions> Couples { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public (string BotToken, string ChatId)? ResolveForWebId(int webId)
    {
        string? bot = null;
        string? chat = null;

        if (webId > 0 &&
            Couples.TryGetValue(webId.ToString(), out var couple) &&
            couple is not null)
        {
            if (IsUsableToken(couple.BotToken))
                bot = couple.BotToken.Trim();
            if (IsUsableChatId(couple.ChatId))
                chat = couple.ChatId.Trim();
        }

        if (!IsUsableToken(bot) && IsUsableToken(BotToken))
            bot = BotToken.Trim();
        if (!IsUsableChatId(chat) && IsUsableChatId(ChatId))
            chat = ChatId.Trim();

        if (!IsUsableToken(bot) || !IsUsableChatId(chat))
            return null;

        return (bot!, chat!);
    }

    private static bool IsUsableToken(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        value.Contains(':', StringComparison.Ordinal) &&
        !value.Contains("REPLACE", StringComparison.OrdinalIgnoreCase);

    private static bool IsUsableChatId(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        if (value.Contains("REPLACE", StringComparison.OrdinalIgnoreCase)) return false;
        var s = value.Trim();
        if (s.StartsWith('-')) s = s[1..];
        return s.Length > 0 && s.All(char.IsDigit);
    }
}

public class TelegramCoupleOptions
{
    /// <summary>Optional bot override for this Web ID. Leave empty to use default BotToken.</summary>
    public string BotToken { get; set; } = "";

    /// <summary>Chat / group / channel that receives RSVPs for this Web ID.</summary>
    public string ChatId { get; set; } = "";
}
