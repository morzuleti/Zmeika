using System.Collections.Frozen;
using System.Globalization;
using Zmeika.App.Rendering;

namespace Zmeika.App.Input;

internal static class KeyNames
{
    private static readonly FrozenDictionary<ConsoleKey, string> s_special = new Dictionary<ConsoleKey, string>
    {
        [ConsoleKey.UpArrow] = Glyphs.ArrowUp.ToString(),
        [ConsoleKey.DownArrow] = Glyphs.ArrowDown.ToString(),
        [ConsoleKey.LeftArrow] = Glyphs.ArrowLeft.ToString(),
        [ConsoleKey.RightArrow] = Glyphs.ArrowRight.ToString(),
        [ConsoleKey.Spacebar] = "Space",
    }.ToFrozenDictionary();

    public static HudControls ForHud(KeyBindings keys)
    {
        ArgumentNullException.ThrowIfNull(keys);

        return new HudControls(
            Display(keys.Up),
            Display(keys.Down),
            Display(keys.Left),
            Display(keys.Right),
            Display(ConsoleKey.Spacebar));
    }

    public static string Display(ConsoleKey key)
    {
        if (s_special.TryGetValue(key, out string? special))
        {
            return special;
        }

        if (key is >= ConsoleKey.D0 and <= ConsoleKey.D9)
        {
            return (key - ConsoleKey.D0).ToString(CultureInfo.InvariantCulture);
        }

        if (key is >= ConsoleKey.NumPad0 and <= ConsoleKey.NumPad9)
        {
            return string.Create(CultureInfo.InvariantCulture, $"Num{key - ConsoleKey.NumPad0}");
        }

        return key.ToString();
    }
}
