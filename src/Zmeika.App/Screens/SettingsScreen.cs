using Zmeika.App.Input;
using Zmeika.App.Rendering;
using Zmeika.App.Storage;
using Zmeika.Core.Gameplay;
using Zmeika.Core.Model;

namespace Zmeika.App.Screens;

internal sealed class SettingsScreen(GameContext context, GameSession session, IScreen returnTo) : IScreen
{
    private const int Width = 30;
    private const int Height = 13;
    private const int ItemsOffsetY = 3;
    private const int MarkerOffsetX = 3;
    private const int LabelOffsetX = 5;
    private const int ValueOffsetX = 13;
    private const int SkinItem = 0;
    private const int ColorItem = 1;
    private const int FirstKeyItem = 2;
    private const int BackItem = 6;

    private static readonly string[] s_labels = ["Skin", "Colour", "Up", "Down", "Left", "Right", "Back"];
    private static readonly Direction[] s_directions = [Direction.Up, Direction.Down, Direction.Left, Direction.Right];
    private static readonly TimeSpan s_messageDuration = TimeSpan.FromSeconds(2);

    private readonly MenuSelection _selection = new(s_labels.Length);
    private Direction? _capturing;
    private string? _message;
    private TimeSpan _messageLeft;

    public void HandleKey(ConsoleKeyInfo key)
    {
        if (_capturing is { } direction)
        {
            Capture(direction, key.Key);
            return;
        }

        if (GameKeys.IsPause(key))
        {
            context.Navigator.Show(returnTo);
            return;
        }

        if (_selection.TryMove(key))
        {
            return;
        }

        int item = _selection.Index;
        switch (key.Key)
        {
            case ConsoleKey.LeftArrow or ConsoleKey.RightArrow when item is SkinItem or ColorItem:
                ChangeValue(item, key.Key == ConsoleKey.RightArrow ? 1 : -1);
                break;
            case ConsoleKey.Enter when item == BackItem:
                context.Navigator.Show(returnTo);
                break;
            case ConsoleKey.Enter when item >= FirstKeyItem:
                _capturing = s_directions[item - FirstKeyItem];
                break;
            default:
                break;
        }
    }

    public void Update(TimeSpan elapsed)
    {
        if (_messageLeft > TimeSpan.Zero)
        {
            _messageLeft -= elapsed;
        }
    }

    public void Paint(FrameBuffer buffer)
    {
        LevelView.Paint(buffer, context, session);

        MenuTheme menu = context.Theme.Menu;
        int x = ScreenLayout.CenterX(ScreenLayout.FieldX, ScreenLayout.FieldWidth, Width);
        int y = LevelView.DialogY(Height);
        DialogPainter.Paint(buffer, x, y, Width, Height, "SETTINGS", menu);

        for (var i = 0; i < s_labels.Length; i++)
        {
            int row = y + ItemsOffsetY + i;
            bool isSelected = i == _selection.Index;
            if (isSelected)
            {
                buffer.Put(x + MarkerOffsetX, row, Glyphs.MenuMarker, menu.Marker);
            }

            buffer.Write(x + LabelOffsetX, row, s_labels[i], isSelected ? menu.SelectedItem : menu.Item);
            PaintValue(buffer, x + ValueOffsetX, row, i);
        }

        buffer.Write(x + 2, y + Height - 2, Hint(), menu.Hint);
    }

    private void PaintValue(FrameBuffer buffer, int x, int y, int item)
    {
        MenuTheme menu = context.Theme.Menu;
        switch (item)
        {
            case SkinItem:
                SnakeSkin skin = context.Skin;
                buffer.Write(x, y, $"{Glyphs.ValuePrevious} {skin.Name} {skin.Head}{skin.Body} {Glyphs.ValueNext}", menu.Item);
                buffer.Put(x + 3 + skin.Name.Length, y, skin.Head, context.SnakeColor);
                buffer.Put(x + 4 + skin.Name.Length, y, skin.Body, context.SnakeColor);
                break;
            case ColorItem:
                buffer.Write(x, y, $"{Glyphs.ValuePrevious} {context.Settings.SnakeColor} {Glyphs.ValueNext}", menu.Item);
                buffer.Write(x + 2, y, context.Settings.SnakeColor, context.SnakeColor);
                break;
            case BackItem:
                break;
            default:
                Direction direction = s_directions[item - FirstKeyItem];
                string value = _capturing == direction ? "press a key" : KeyNames.Display(context.Keys.KeyFor(direction));
                buffer.Write(x, y, value, _capturing == direction ? menu.InputCursor : menu.Item);
                break;
        }
    }

    private string Hint()
    {
        if (_message is not null && _messageLeft > TimeSpan.Zero)
        {
            return _message;
        }

        return _capturing is not null
            ? "Press a key  Esc cancel"
            : _selection.Index switch
            {
                SkinItem or ColorItem => $"{Glyphs.ArrowLeft}{Glyphs.ArrowRight} change  Esc back",
                BackItem => "Enter ok  Esc back",
                _ => "Enter change key  Esc back",
            };
    }

    private void ChangeValue(int item, int step)
    {
        Theme theme = context.Theme;
        PlayerSettings current = context.Settings;
        PlayerSettings updated = item == SkinItem
            ? current with { SnakeSkin = Cycle([.. theme.Skins.Select(skin => skin.Name)], current.SnakeSkin, step) }
            : current with { SnakeColor = Cycle([.. theme.SnakeColors.Select(color => color.Name)], current.SnakeColor, step) };
        Save(updated);
    }

    private void Capture(Direction direction, ConsoleKey key)
    {
        if (key == ConsoleKey.Escape)
        {
            _capturing = null;
            return;
        }

        KeyBindings keys = context.Keys;
        if (KeyBindings.IsReserved(key))
        {
            ShowMessage($"{KeyNames.Display(key)} is reserved");
            return;
        }

        foreach (Direction other in s_directions)
        {
            if (other != direction && keys.KeyFor(other) == key)
            {
                ShowMessage($"{KeyNames.Display(key)} is used for {other}");
                return;
            }
        }

        _capturing = null;
        Save(context.Settings with { Keys = keys.With(direction, key) });
    }

    private void Save(PlayerSettings settings)
    {
        try
        {
            context.SettingsStore.Save(settings);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            ShowMessage("Could not save settings");
        }
    }

    private void ShowMessage(string message)
    {
        _message = message;
        _messageLeft = s_messageDuration;
    }

    private static string Cycle(string[] values, string current, int step)
    {
        int index = Array.FindIndex(values, value => string.Equals(value, current, StringComparison.OrdinalIgnoreCase));
        int next = (Math.Max(index, 0) + step + values.Length) % values.Length;
        return values[next];
    }
}
