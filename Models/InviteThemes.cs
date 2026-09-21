namespace WeddingManagementSystem.Models;

public static class InviteThemes
{
    public record ThemeInfo(int Id, string Name, string Description, string Primary, string Accent, string Bg, string Card);

    public static readonly IReadOnlyList<ThemeInfo> All =
    [
        new(1, "Layout 1", "Rose & cream — soft classic", "#9c4d5e", "#c97b8a", "#faf4f2", "#fffdfb"),
        new(2, "Layout 2", "Sage & teal — garden fresh", "#3d6b5a", "#5a9a82", "#f2f7f4", "#fcfffe"),
        new(3, "Layout 3", "Midnight & gold — evening formal", "#1c2430", "#c9a36a", "#121820", "#1e2633"),
        new(4, "Layout 4", "Red fresh — bold & vibrant", "#b91c1c", "#ef4444", "#fff5f5", "#ffffff"),
        new(5, "Layout 5", "Pink fresh — bright & playful", "#db2777", "#f472b6", "#fdf2f8", "#ffffff"),
        new(6, "Layout 6", "Pink classic — soft & timeless", "#9f1239", "#e11d48", "#fff1f2", "#fffbeb"),
        new(7, "Layout 7", "Red classic — elegant & formal", "#7f1d1d", "#b91c1c", "#fef2f2", "#fffaf5")
    ];

    public static ThemeInfo Get(int id) =>
        All.FirstOrDefault(t => t.Id == id) ?? All[5];
}
