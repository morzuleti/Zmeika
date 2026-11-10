using Zmeika.App.Input;
using Zmeika.App.Rendering;
using Zmeika.Core.Gameplay;

namespace Zmeika.App.Screens;

internal static class LevelView
{
    public const int DialogWidth = 24;

    public static int DialogX => ScreenLayout.CenterX(ScreenLayout.FieldX, ScreenLayout.FieldWidth, DialogWidth);

    public static void Paint(FrameBuffer buffer, GameContext context, GameSession session)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(session);

        Theme theme = context.Theme;
        LevelSession level = session.Current;
        FieldStyle style = new(
            theme.Levels[level.Level - 1],
            theme.DefaultSkin,
            theme.DefaultSnakeColor.Color,
            theme.AppleColors,
            theme.Heart);
        FieldPainter.Paint(buffer, level, style);

        HudModel hud = new(
            level.Level,
            session.Rules.LevelCount,
            context.PlayerName,
            session.Lives,
            level.ApplesEaten,
            level.ApplesRequired,
            session.Score,
            KeyNames.ForHud(context.Keys));
        HudPainter.Paint(buffer, hud, theme.Hud);
    }

    public static int DialogY(int height) => ScreenLayout.FieldY + ((ScreenLayout.FieldHeight - height) / 2);
}
