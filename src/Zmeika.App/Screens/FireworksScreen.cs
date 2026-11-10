using Zmeika.App.Input;
using Zmeika.App.Rendering;

namespace Zmeika.App.Screens;

internal sealed class FireworksScreen(
    FireworksSimulation fireworks,
    FireworksTheme theme,
    TimeSpan duration,
    Action<FrameBuffer> paintCaption,
    Action onFinished) : IScreen
{
    private TimeSpan _elapsed;
    private bool _isFinished;

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
        fireworks.Update(elapsed);
        if (_elapsed >= duration)
        {
            Finish();
        }
    }

    public void Paint(FrameBuffer buffer)
    {
        FireworksPainter.Paint(buffer, fireworks, theme);
        paintCaption(buffer);
    }

    private void Finish()
    {
        if (!_isFinished)
        {
            _isFinished = true;
            onFinished();
        }
    }
}
