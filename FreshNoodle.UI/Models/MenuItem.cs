namespace FreshNoodle.UI.Models;

public class MenuItem
{
    public string Title { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string? Roles { get; set; } // Comma-separated roles, null for all authenticated
    public bool RequireAuth { get; set; } = true;
}
