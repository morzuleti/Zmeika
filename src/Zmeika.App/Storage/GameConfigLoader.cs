using System.Text.Json;
using Zmeika.App.Storage.Dto;

namespace Zmeika.App.Storage;

internal static class GameConfigLoader
{
    public const string RelativePath = "config/game.json";

    private const string EmbeddedResourceName = "Zmeika.App.config.game.json";
    private const string EmbeddedSourceName = "built-in game.json";

    public static GameConfig Load(string baseDirectory)
    {
        ArgumentNullException.ThrowIfNull(baseDirectory);

        string path = Path.Combine(baseDirectory, RelativePath);
        (string json, string sourceName) = File.Exists(path)
            ? (File.ReadAllText(path), RelativePath)
            : (ReadEmbeddedDefaults(), EmbeddedSourceName);

        try
        {
            GameConfigDto document = Parse(json);
            return new GameConfig(document, ThemeFactory.Create(document), RulesFactory.Create(document));
        }
        catch (Exception exception) when (exception is JsonException or ConfigException)
        {
            throw new ConfigException($"{sourceName}: {exception.Message}", exception);
        }
    }

    private static GameConfigDto Parse(string json) =>
        JsonSerializer.Deserialize(json, StorageJsonContext.Default.GameConfigDto)
        ?? throw new ConfigException("the file is empty.");

    private static string ReadEmbeddedDefaults()
    {
        using Stream stream = typeof(GameConfigLoader).Assembly.GetManifestResourceStream(EmbeddedResourceName)
            ?? throw new InvalidOperationException($"Embedded resource '{EmbeddedResourceName}' is missing.");
        using StreamReader reader = new(stream);
        return reader.ReadToEnd();
    }
}
