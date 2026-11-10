using System.Text;
using Zmeika.App.Input;
using Zmeika.App.Rendering;
using Zmeika.Core.Players;

namespace Zmeika.App.Screens;

internal sealed class NameInputScreen(GameContext context) : IScreen
{
    private const int Height = 7;
    private const int TextOffsetX = 4;

    private static readonly TimeSpan s_cursorBlinkHalfPeriod = TimeSpan.FromMilliseconds(500);

    private readonly StringBuilder _text = new();
    private TimeSpan _elapsed;

    public void HandleKey(ConsoleKeyInfo key)
    {
        if (GameKeys.IsConfirm(key))
        {
            Confirm();
        }
        else if (key.Key == ConsoleKey.Backspace)
        {
            if (_text.Length > 0)
            {
                _text.Length--;
            }
        }
        else if (IsTypable(key.KeyChar) && _text.Length < context.Interface.MaxNameLength)
        {
            _text.Append(key.KeyChar);
        }
    }

    public void Update(TimeSpan elapsed) => _elapsed += elapsed;

    public void Paint(FrameBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        MenuTheme menu = context.Theme.Menu;
        int x = ScreenLayout.CenterX(0, buffer.Width, LevelView.DialogWidth);
        int y = (buffer.Height - Height) / 2;
        DialogPainter.Paint(buffer, x, y, LevelView.DialogWidth, Height, "ENTER YOUR NAME", menu);

        string text = _text.ToString();
        buffer.Write(x + TextOffsetX, y + 3, text, menu.InputText);
        if ((long)(_elapsed / s_cursorBlinkHalfPeriod) % 2 == 0 && _text.Length < context.Interface.MaxNameLength)
        {
            buffer.Put(x + TextOffsetX + text.Length, y + 3, Glyphs.InputCursor, menu.InputCursor);
        }

        buffer.Write(x + 2, y + 5, "Enter ok", menu.Hint);
    }

    private static bool IsTypable(char character) => character != '\0' && !char.IsControl(character);

    private void Confirm()
    {
        if (!PlayerName.TryCreate(_text.ToString(), context.Interface.MaxNameLength, out PlayerName? player))
        {
            return;
        }

        if (context.Flow.FindProgress(player) is { } progress)
        {
            context.Navigator.Show(new WelcomeBackScreen(context, player, progress));
            return;
        }

        context.Navigator.Show(new GetReadyScreen(context, context.Flow.StartNewGame(player)));
    }
}
