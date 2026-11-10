using System.Text.Json;
using Zmeika.App.Input;
using Zmeika.App.Rendering;
using Zmeika.App.Storage.Dto;

namespace Zmeika.App.Storage;

internal sealed class SettingsStore
{
    public const string RelativePath = "data/settings.json";

    private readonly string _path;

    private SettingsStore(string path, PlayerSettings current)
    {
        _path = path;
        Current = current;
    }

    public PlayerSettings Current { get; private set; }

    public static SettingsStore Load(string baseDirectory, Theme theme)
    {
        ArgumentNullException.ThrowIfNull(baseDirectory);
        ArgumentNullException.ThrowIfNull(theme);

        string path = Path.Combine(baseDirectory, RelativePath);
        PlayerSettings? loaded = TryRead(path, theme);
        SettingsStore store = new(path, loaded ?? PlayerSettings.Default(theme));
        if (loaded is null)
        {
            store.TrySave(store.Current);
        }

        return store;
    }

    public void Save(PlayerSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        JsonFiles.WriteAtomic(_path, ToDto(settings), StorageJsonContext.Default.SettingsDto);
        Current = settings;
    }

    private static PlayerSettings? TryRead(string path, Theme theme)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            SettingsDto? dto = JsonFiles.Read(path, StorageJsonContext.Default.SettingsDto);
            return dto is null ? null : FromDto(dto, theme);
        }
        catch (Exception exception) when (exception is JsonException or IOException or UnauthorizedAccessException)
        {
            return null;
        }
    }

    private static PlayerSettings? FromDto(SettingsDto dto, Theme theme)
    {
        if (!TryParseKey(dto.Keys.Up, out ConsoleKey up)
            || !TryParseKey(dto.Keys.Down, out ConsoleKey down)
            || !TryParseKey(dto.Keys.Left, out ConsoleKey left)
            || !TryParseKey(dto.Keys.Right, out ConsoleKey right))
        {
            return null;
        }

        PlayerSettings settings = new(dto.SnakeSkin, dto.SnakeColor, new KeyBindings(up, down, left, right));
        return settings.IsValidFor(theme) ? settings : null;
    }

    private static bool TryParseKey(string name, out ConsoleKey key) =>
        Enum.TryParse(name, ignoreCase: true, out key) && Enum.IsDefined(key) && !int.TryParse(name, out _);

    private static SettingsDto ToDto(PlayerSettings settings) => new(
        settings.SnakeSkin,
        settings.SnakeColor,
        new KeysDto(
            settings.Keys.Up.ToString(),
            settings.Keys.Down.ToString(),
            settings.Keys.Left.ToString(),
            settings.Keys.Right.ToString()));

    private void TrySave(PlayerSettings settings)
    {
        try
        {
            Save(settings);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            Current = settings;
        }
    }
}
