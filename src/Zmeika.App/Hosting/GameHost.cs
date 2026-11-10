using System.Diagnostics;
using Zmeika.App.ConsoleHost;
using Zmeika.App.Input;
using Zmeika.App.Rendering;
using Zmeika.App.Screens;

namespace Zmeika.App.Hosting;

internal sealed class GameHost(ConsoleSession console) : INavigator
{
    private static readonly TimeSpan s_frameInterval = TimeSpan.FromMilliseconds(33);
    private static readonly TimeSpan s_maxFrameElapsed = TimeSpan.FromMilliseconds(250);

    private readonly FrameBuffer _buffer = new(ScreenLayout.Width, ScreenLayout.Height);
    private readonly ConsoleRenderer _renderer = new(ScreenLayout.Width, ScreenLayout.Height);
    private IScreen? _pending;
    private bool _exitRequested;

    public void Show(IScreen screen)
    {
        ArgumentNullException.ThrowIfNull(screen);
        _pending = screen;
    }

    public void Exit() => _exitRequested = true;

    public async Task RunAsync(IScreen firstScreen)
    {
        ArgumentNullException.ThrowIfNull(firstScreen);

        IScreen current = firstScreen;
        current.OnEnter();

        using PeriodicTimer timer = new(s_frameInterval);
        Stopwatch clock = Stopwatch.StartNew();
        TimeSpan previousTime = clock.Elapsed;

        while (!_exitRequested)
        {
            TimeSpan now = clock.Elapsed;
            TimeSpan elapsed = now - previousTime < s_maxFrameElapsed ? now - previousTime : s_maxFrameElapsed;
            previousTime = now;

            foreach (ConsoleKeyInfo key in KeyReader.ReadAvailable())
            {
                current.HandleKey(key);
            }

            current.Update(elapsed);
            current = SwitchIfRequested(current);

            if (console.EnsureSize())
            {
                _renderer.Invalidate();
            }

            _buffer.Clear();
            current.Paint(_buffer);
            _renderer.Present(_buffer);

            await timer.WaitForNextTickAsync().ConfigureAwait(false);
        }

        current.OnLeave();
    }

    private IScreen SwitchIfRequested(IScreen current)
    {
        if (_pending is not { } next)
        {
            return current;
        }

        _pending = null;
        current.OnLeave();
        next.OnEnter();
        _renderer.Invalidate();
        return next;
    }
}
