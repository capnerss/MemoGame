namespace MemoGame.Models;

public class Theme
{
	public string Name { get; }
	public Color BackgroundColor { get; }
	public Color CardClosedColor { get; }
	public Color CardOpenColor { get; }
	public Color TextColor { get; }
	public string FontFamily { get; }

	public Theme(string name, Color bg, Color cardClosed, Color cardOpen, Color text, string font = "OpenSansRegular")
	{
		Name = name;
		BackgroundColor = bg;
		CardClosedColor = cardClosed;
		CardOpenColor = cardOpen;
		TextColor = text;
		FontFamily = font;
	}

	public void Apply(ContentPage page)
	{
		page.Resources["PageBackground"] = BackgroundColor;
		page.Resources["CardClosedBrush"] = CardClosedColor;
		page.Resources["CardOpenBrush"] = CardOpenColor;
		page.Resources["TextColorBrush"] = TextColor;
		page.Resources["DefaultFont"] = FontFamily;
	}

	public static List<Theme> GetAvailableThemes() => new()
	{
		new Theme("Hele (Light)",
			Color.FromArgb("#F4F6F9"),
			Color.FromArgb("#3B82F6"),
			Color.FromArgb("#FFFFFF"),
			Color.FromArgb("#1E293B")),

		new Theme("Tume (Dark)",
			Color.FromArgb("#0F172A"),
			Color.FromArgb("#1E293B"),
			Color.FromArgb("#334155"),
			Color.FromArgb("#F8FAFC")),

		new Theme("Värviline (Emerald)",
			Color.FromArgb("#ECFDF5"),
			Color.FromArgb("#059669"),
			Color.FromArgb("#D1FAE5"),
			Color.FromArgb("#064E3B"))
	};
}