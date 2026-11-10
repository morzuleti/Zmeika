using System.Globalization;
using Zmeika.App.Rendering;

namespace Zmeika.App.Preview.Pages;

internal sealed class InterfacePage(Theme theme) : IPreviewPage
{
    private const int DialogWidth = 24;
    private const int ItemOffsetX = 5;

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

    private void DrawDialog(FrameBuffer buffer, int x, int y, int height, string title)
    {
        FrameDrawing.Draw(buffer, x, y, DialogWidth, height, FrameStyle.Double, Menu.Frame);
        buffer.Write(ScreenLayout.CenterX(x, DialogWidth, title.Length), y + 1, title, Menu.Title);
    }

    private void DrawItem(FrameBuffer buffer, int x, int y, string text, bool isSelected)
    {
        if (isSelected)
        {
            buffer.Put(x + ItemOffsetX - 2, y, Glyphs.MenuMarker, Menu.Marker);
        }

        buffer.Write(x + ItemOffsetX, y, text, isSelected ? Menu.SelectedItem : Menu.Item);
    }

    private void DrawMainMenu(FrameBuffer buffer, int x, int y)
    {
        DrawDialog(buffer, x, y, 10, "MENU");
        DrawItem(buffer, x, y + 3, "Continue", isSelected: true);
        DrawItem(buffer, x, y + 4, "New game", isSelected: false);
        DrawItem(buffer, x, y + 5, "Settings", isSelected: false);
        DrawItem(buffer, x, y + 6, "Exit", isSelected: false);
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
        DrawItem(buffer, x, y + 4, "Continue", isSelected: true);
        DrawItem(buffer, x, y + 5, "New game", isSelected: false);
        buffer.Write(x + 2, y + 8, $"{Glyphs.ArrowUp}{Glyphs.ArrowDown} choose  Enter ok", Menu.Hint);
    }

    private void DrawLeaderboard(FrameBuffer buffer, int x, int y)
    {
        LeaderboardTheme leaderboard = theme.Leaderboard;
        buffer.Write(x, y, "LEADERBOARD", leaderboard.Title);
        buffer.Write(x + 2, y + 2, "#   Name                 Score", leaderboard.Header);
        WriteScore(buffer, x, y + 3, 1, "Petya", 2750, leaderboard.FirstPlace, isCurrent: false);
        WriteScore(buffer, x, y + 4, 2, "Masha", 1200, leaderboard.CurrentGame, isCurrent: true);
        WriteScore(buffer, x, y + 5, 3, "Kolya", 600, leaderboard.OtherGames, isCurrent: false);
        WriteScore(buffer, x, y + 6, 4, "Dasha", 350, leaderboard.OtherGames, isCurrent: false);
        string legend =
            $"1st place {theme.Describe(leaderboard.FirstPlace)}, " +
            $"current game {theme.Describe(leaderboard.CurrentGame)}, " +
            $"others {theme.Describe(leaderboard.OtherGames)}";
        buffer.Write(x, y + 8, legend, leaderboard.Header);
    }

    private static void WriteScore(FrameBuffer buffer, int x, int y, int place, string name, int score, Rgb color, bool isCurrent)
    {
        if (isCurrent)
        {
            buffer.Put(x, y, Glyphs.MenuMarker, color);
        }

        buffer.Write(x + 2, y, string.Create(CultureInfo.InvariantCulture, $"{place,-3} {name,-18} {score,6}"), color);
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
