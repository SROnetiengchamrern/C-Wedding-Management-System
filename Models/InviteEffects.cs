namespace WeddingManagementSystem.Models;

public static class InviteEffects
{
    public record EffectInfo(int Id, string Name, string Description);

    public const int None = 0;
    public const int HeartsSnow = 1;
    public const int HeartsOnly = 2;
    public const int SoftSnow = 3;
    public const int RosePetals = 4;
    public const int GoldenSparkles = 5;

    public static readonly IReadOnlyList<EffectInfo> All =
    [
        new(None, "None", "No falling effect (heart loader only)"),
        new(HeartsSnow, "Hearts + snow", "Pink hearts with soft snowflakes"),
        new(HeartsOnly, "Hearts only", "Falling hearts only"),
        new(SoftSnow, "Soft snow", "Light snowflakes only"),
        new(RosePetals, "Rose petals", "Soft petal shapes drifting down"),
        new(GoldenSparkles, "Golden sparkles", "Warm sparkles and dots")
    ];

    public static EffectInfo Get(int id) =>
        All.FirstOrDefault(e => e.Id == id) ?? All[0];

    public static bool IsValid(int id) => All.Any(e => e.Id == id);
}
