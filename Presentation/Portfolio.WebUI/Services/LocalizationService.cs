using System.Text.Json;

public class LocalizationService
{
    private readonly string _language;
    private readonly Dictionary<string, string> _translations;

    public LocalizationService(string language)
    {
        _language = language;
        var filePath = Path.Combine("Resources", "Languages", $"{_language}.json");
        var json = File.ReadAllText(filePath);
        _translations = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? null!;
    }

    public string Get(string key)
    {
        return _translations.ContainsKey(key) ? _translations[key] : key;
    }
}