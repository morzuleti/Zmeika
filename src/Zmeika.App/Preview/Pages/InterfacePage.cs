using Zmeika.App.Rendering;
using Zmeika.Core.Players;

namespace Zmeika.App.Preview.Pages;

internal sealed class InterfacePage(Theme theme) : IPreviewPage
{
    private const int DialogWidth = 24;

    private MenuTheme Menu => theme.Menu;

    public string Title => "Menus, name input, leaderboard, key names";

    public void Draw(FrameBuffer buffer)
    {
        DrawMainMenu(buffer, 2, 2);
        DrawNameInput(buffer, 32, 2);
        DrawWelcomeBack(buffer, 62, 2);
        DrawLeaderboard(buffer, 4, 15);
        DrawKeyNames(buffer, 50, 15);
    }

    private void DrawDialog(FrameBuffer buffer, int x, int y, int height, string title) =>
        DialogPainter.Paint(buffer, x, y, DialogWidth, height, title, Menu);

    private void DrawMainMenu(FrameBuffer buffer, int x, int y)
    {
        DrawDialog(buffer, x, y, 10, "MENU");
        MenuPainter.Paint(buffer, x, y + 3, ["Continue", "New game", "Settings", "Exit"], 0, Menu);
        buffer.Write(x + 2, y + 8, $"{Glyphs.ArrowUp}{Glyphs.ArrowDown} choose  Enter ok", Menu.Hint);
    }

    private void DrawNameInput(FrameBuffer buffer, int x, int y)
    {
        DrawDialog(buffer, x, y, 7, "ENTER YOUR NAME");
        buffer.Write(x + 4, y + 3, "Masha", Menu.InputText);
        buffer.Put(x + 9, y + 3, Glyphs.InputCursor, Menu.InputCursor);
        buffer.Write(x + 2, y + 5, "Enter ok", Menu.Hint);
    }

    private void DrawWelcomeBack(FrameBuffer buffer, int x, int y)
    {
        DrawDialog(buffer, x, y, 10, "WELCOME BACK");
        buffer.Write(ScreenLayout.CenterX(x, DialogWidth, 5), y + 2, "Masha", Menu.PlayerName);
        MenuPainter.Paint(buffer, x, y + 4, ["Continue", "New game"], 0, Menu);
        buffer.Write(x + 2, y + 8, $"{Glyphs.ArrowUp}{Glyphs.ArrowDown} choose  Enter ok", Menu.Hint);
    }

    private void DrawLeaderboard(FrameBuffer buffer, int x, int y)
    {
        LeaderboardTheme leaderboard = theme.Leaderboard;
        buffer.Write(x, y, "LEADERBOARD", leaderboard.Title);
        LeaderboardRow[] rows =
        [
            new(1, "Petya", 2750, IsCurrent: false),
            new(2, "Masha", 1200, IsCurrent: true),
            new(3, "Kolya", 600, IsCurrent: false),
            new(4, "Dasha", 350, IsCurrent: false),
        ];
        LeaderboardPainter.Paint(buffer, x, y + 2, rows, leaderboard);
        string legend =
            $"1st place {theme.Describe(leaderboard.FirstPlace)}, " +
            $"current game {theme.Describe(leaderboard.CurrentGame)}, " +
            $"others {theme.Describe(leaderboard.OtherGames)}";
        buffer.Write(x, y + 8, legend, leaderboard.Header);
    }


    private void DrawKeyNames(FrameBuffer buffer, int x, int y)
    {
        buffer.Write(x, y, "Key names in HUD", theme.Hud.ControlsCaption);
        WriteKey(buffer, x, y + 2, "Arrow keys", $"{Glyphs.ArrowUp} {Glyphs.ArrowDown} {Glyphs.ArrowLeft} {Glyphs.ArrowRight}");
        WriteKey(buffer, x, y + 3, "Letters, digits", "W  A  S  D  8");
        WriteKey(buffer, x, y + 4, "Numpad", "Num8  Num4");
        WriteKey(buffer, x, y + 5, "Other keys", "Enter  Tab  Home");
    }

    private void WriteKey(FrameBuffer buffer, int x, int y, string label, string keys)
    {
        buffer.Write(x, y, label, theme.Hud.ControlDirection);
        buffer.Write(x + 17, y, keys, theme.Hud.ControlKey);
    }
}
