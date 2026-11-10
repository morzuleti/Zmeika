using System.Globalization;
using Zmeika.App.Input;
using Zmeika.App.Rendering;
using Zmeika.Core.Gameplay;

namespace Zmeika.App.Screens;

internal sealed class LevelCompleteScreen : IScreen
{
    private readonly GameContext _context;
    private readonly GameSession _session;
    private readonly FireworksSimulation _fireworks;
    private TimeSpan _elapsed;
    private bool _isFinished;

    public LevelCompleteScreen(GameContext context, GameSession session)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(session);

        _context = context;
        _session = session;
        _fireworks = new FireworksSimulation(
            FireworksStyle.LevelComplete,
            context.Theme.Fireworks.LevelBurstColors,
            new Random(),
            ScreenLayout.Width,
            ScreenLayout.Height);
    }

    public void HandleKey(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Spacebar || GameKeys.IsConfirm(key))
        {
            Finish();
        }
    }

    public void Update(TimeSpan elapsed)
    {
        _elapsed += elapsed;
        _fireworks.Update(elapsed);
        if (_elapsed >= _context.Timings.LevelFireworks)
        {
            Finish();
        }
    }

    public void Paint(FrameBuffer buffer)
    {
        ArgumentNullException.ThrowIfNull(buffer);

        FireworksPainter.Paint(buffer, _fireworks, _context.Theme.Fireworks);
        string caption = string.Create(CultureInfo.InvariantCulture, $"LEVEL {_session.Level} COMPLETE!");
        buffer.Write(ScreenLayout.CenterX(0, buffer.Width, caption.Length), 1, caption, _context.Theme.Fireworks.LevelCaption);
    }

    private void Finish()
    {
        if (_isFinished)
        {
            return;
        }

        _isFinished = true;
        _session.AdvanceToNextLevel();
        _context.Navigator.Show(new GetReadyScreen(_context, _session));
    }
}
