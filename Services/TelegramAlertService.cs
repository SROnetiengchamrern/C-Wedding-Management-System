using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Options;

namespace WeddingManagementSystem.Services;

public interface ITelegramAlertService
{
    Task SendRsvpAlertAsync(
        string coupleName,
        int webId,
        string slug,
        string guestName,
        string reply,
        string? email,
        string? message,
        string? dietaryNotes,
        CancellationToken ct = default);
}

public class TelegramAlertService : ITelegramAlertService
{
    private readonly HttpClient _http;
    private readonly TelegramOptions _options;
    private readonly ILogger<TelegramAlertService> _logger;

    public TelegramAlertService(
        HttpClient http,
        IOptions<TelegramOptions> options,
        ILogger<TelegramAlertService> logger)
    {
        _http = http;
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendRsvpAlertAsync(
        string coupleName,
        int webId,
        string slug,
        string guestName,
        string reply,
        string? email,
        string? message,
        string? dietaryNotes,
        CancellationToken ct = default)
    {
        if (!_options.Enabled ||
            string.IsNullOrWhiteSpace(_options.BotToken) ||
            string.IsNullOrWhiteSpace(_options.ChatId))
        {
            _logger.LogDebug("Telegram RSVP alert skipped (disabled or missing config).");
            return;
        }

        var replyLabel = reply switch
        {
            "Accepted" => "✅ Accepted",
            "Declined" => "❌ Declined",
            "Maybe" => "🤔 Maybe",
            _ => reply
        };

        var sb = new StringBuilder();
        sb.AppendLine("💌 <b>New wedding RSVP</b>");
        sb.AppendLine();
        sb.AppendLine($"<b>Couple:</b> {Escape(coupleName)} (Web ID {webId})");
        sb.AppendLine($"<b>Site:</b> /w/{Escape(slug)}");
        sb.AppendLine($"<b>Guest:</b> {Escape(guestName)}");
        sb.AppendLine($"<b>Reply:</b> {Escape(replyLabel)}");
        if (!string.IsNullOrWhiteSpace(email))
            sb.AppendLine($"<b>Email:</b> {Escape(email)}");
        if (!string.IsNullOrWhiteSpace(message))
            sb.AppendLine($"<b>Message:</b> {Escape(message)}");
        if (!string.IsNullOrWhiteSpace(dietaryNotes))
            sb.AppendLine($"<b>Dietary:</b> {Escape(dietaryNotes)}");
        sb.AppendLine();
        sb.AppendLine($"<i>{DateTime.Now:yyyy-MM-dd HH:mm}</i>");

        var url = $"https://api.telegram.org/bot{_options.BotToken.Trim()}/sendMessage";
        var payload = new
        {
            chat_id = _options.ChatId.Trim(),
            text = sb.ToString(),
            parse_mode = "HTML",
            disable_web_page_preview = true
        };

        try
        {
            using var response = await _http.PostAsJsonAsync(url, payload, ct);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(ct);
                _logger.LogWarning("Telegram send failed ({Status}): {Body}", (int)response.StatusCode, body);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Telegram RSVP alert failed.");
        }
    }

    private static string Escape(string value) =>
        value
            .Replace("&", "&amp;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace(">", "&gt;", StringComparison.Ordinal);
}
